using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI.WebControls;
using System.IO;
using System.Web;

using Brierley.LWModules.AccountActivity.Components;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.IO;
using Brierley.FrameWork.Pdf;

using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Ipc;

namespace Brierley.LWModules.AccountActivity
{
	public partial class ViewAccountActivity : ModuleControlBase, IIpcEventHandler
	{
		private const string _className = "ViewAccountActivity";
		private const string _modulePath = "~/Controls/Modules/AccountActivity/ViewAccountActivity.ascx";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private AccountActivityConfiguration _config = null;
		private IDynamicGridProvider _salesTxnGridProvider = null;
		private AspDynamicGrid _gridSalesTxn = null;
        private IDynamicGridProvider _orphansGridProvider = null;
        private AspDynamicGrid _gridOrphans = null;
		private bool _memberSelected = false;
		private Dictionary<GridView, IDynamicGridProvider> _nestedGrids = new Dictionary<GridView, IDynamicGridProvider>();
        
		private const int _maxDateRanges = 500;

		protected override bool ControlRequiresTelerikSkins()
		{
			return false;
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			const string methodName = "OnInit";
			try
			{
				_memberSelected = PortalState.CurrentMember != null;
				if (!_memberSelected)
				{
					return;
				}

				_config = ConfigurationUtil.GetConfiguration<AccountActivityConfiguration>(ConfigurationKey);

				if (_config == null)
				{
					_logger.Error(_className, methodName, string.Format("Missing configuration for module {0}.", ConfigurationKey));
					return;
                }

				h2Title.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.TitleResourceKey);

				reqFromDate.ValidationGroup = reqToDate.ValidationGroup = btnSearch.ValidationGroup = ValidationGroup;
                customToDateValidator.ValidationGroup = btnSearch.ValidationGroup = ValidationGroup;

                #region Sales txn Grid
                //Sales Txn Grid Details
                lblSalesTxnLabel.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.SalesTxnLabelResourceKey);
				if (string.IsNullOrEmpty(_config.ProviderClassName) || string.IsNullOrEmpty(_config.ProviderAssemblyName))
				{
					_logger.Trace(_className, methodName, "No provider has been set. Using default");
                    _salesTxnGridProvider = new TxnHeaderGridProvider() { ParentControl = _modulePath };
                    _salesTxnGridProvider.SetSearchParm("Config", _config);
				}
				else
				{
                    _salesTxnGridProvider = (IDynamicGridProvider)ClassLoaderUtil.CreateInstance(_config.ProviderAssemblyName, _config.ProviderClassName);
                    if (_salesTxnGridProvider == null)
                    {
                        _logger.Error(_className, methodName,
                            string.Format("Unable to create the grid provider class {0} from assembly {1}",
                            _config.ProviderClassName, _config.ProviderAssemblyName));
                    }
                    _salesTxnGridProvider.ParentControl = _modulePath;
                    _salesTxnGridProvider.SetSearchParm("Config", _config);
				}
                _salesTxnGridProvider.FilteringEnabled = _config.EnableGridFiltering;

				lblNoResults.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.EmptyResultMessageResourceKey);
                #endregion

                #region Orphans Grid
                if (_config.ShowOrphanGrid)
                {
                    // Orphans Txn Grid
                    lblOrphansLabel.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.OrphansLabelResourceKey);
                    if (string.IsNullOrEmpty(_config.OrphansProviderClassName) || string.IsNullOrEmpty(_config.OrphansProviderAssemblyName))
                    {
                        _logger.Trace(_className, methodName, "No provider has been set. Using default");
                        _orphansGridProvider = new OrphanTxnGridProvider() { ParentControl = _modulePath };
                        _orphansGridProvider.SetSearchParm("Config", _config);
                    }
                    else
                    {
                        _orphansGridProvider = (IDynamicGridProvider)ClassLoaderUtil.CreateInstance(_config.OrphansProviderAssemblyName, _config.OrphansProviderClassName);                        
                        if (_orphansGridProvider == null)
                        {
                            _logger.Error(_className, methodName,
                                string.Format("Unable to create the grid provider class {0} from assembly {1}",
                                _config.OrphansProviderClassName, _config.OrphansProviderAssemblyName));
                        }
                        _orphansGridProvider.SetSearchParm("Config", _config);
                        _orphansGridProvider.ParentControl = _modulePath;
                    }
                    _salesTxnGridProvider.FilteringEnabled = _config.EnableGridFiltering;

					lblNoPointResults.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.EmptyResultMessageResourceKey);
                }
                #endregion

                if (IsAjaxPostback())
				{
					try
					{
						_logger.Trace(_className, methodName, "AJAX post begin");
						StringBuilder output = new StringBuilder();
						bool hasSubtotal = false;
						double subtotal = 0;
                        if (_salesTxnGridProvider is INestedGridProvider && Request.Form["keyVal"] != null)
						{
							output.Append("<table>");

                            var childProvider = ((INestedGridProvider)_salesTxnGridProvider).GetChildGridProvider(Request.Form["keyVal"]);
							var specs = childProvider.GetColumnSpecs();
							childProvider.LoadGridData();

							hasSubtotal = (from x in specs where x != null && !string.IsNullOrEmpty(x.SubtotalFormat) select x).Count() > 0;

							if (childProvider.GetNumberOfRows() == 0)
							{
								output.AppendFormat("<tr><td>{0}</td></tr>", ResourceUtils.GetLocalWebResource(_modulePath, _config.EmptyResultMessageResourceKey));
							}
							else
							{
								output.AppendLine("<tr>");
								foreach (var spec in specs)
								{
									if (spec.IsVisible)
									{
										output.Append("<th>");
										output.Append(spec.DisplayText);
										output.Append("</th>");
									}
								}
								output.AppendLine("</tr>");

								for (int i = 0; i < childProvider.GetNumberOfRows(); i++)
								{
									output.AppendLine("<tr>");
									foreach (var spec in specs)
									{
										if (spec.IsVisible)
										{
											object data = childProvider.GetColumnData(i, spec).ToString();
											output.Append("<td>");
                                            output.Append(HttpContext.Current.Server.HtmlEncode(data.ToString()) ?? string.Empty);
											output.Append("</td>");

											if (!string.IsNullOrEmpty(spec.SubtotalFormat))
											{
												subtotal += Convert.ToDouble(data);
											}

										}
									}
									output.AppendLine("</tr>");
								}
								if (hasSubtotal)
								{
									string format = (from x in specs where !string.IsNullOrEmpty(x.SubtotalFormat) select x.SubtotalFormat).FirstOrDefault();
									output.Append("<tr><td class=\"DynamicGridTotal\" colspan=\"" + specs.Length + "\">");
									output.AppendFormat(format, subtotal);
									output.Append("</td></tr>");
								}
							}
							output.Append("</table>");
						}

						Response.ContentType = "application/json; charset=utf-8";
						Response.Clear();
						Response.Write(output.ToString());

						_logger.Trace(_className, methodName, "AJAX post end");
					}
					catch (Exception ex)
					{
						_logger.Error(_className, "AJAX Post", ex.Message, ex);
					}
					finally
					{
						Response.Flush();
						Response.End();
					}
				}
				
				if(!IsAjaxPostback())
				{
					_logger.Trace(_className, methodName, "_config = AccountActivityConfiguration.GetConfiguration(ModuleId); - " + ConfigurationKey);
                   
                    #region Create Sales Txn Grid
                    _gridSalesTxn = new AspDynamicGrid() { AutoRebind = false, EnableViewState = false, CreateTopPanel = false };
					_gridSalesTxn.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
					_gridSalesTxn.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
					_gridSalesTxn.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
					_gridSalesTxn.Provider = _salesTxnGridProvider;
                    if (_salesTxnGridProvider != null && _salesTxnGridProvider is INestedGridProvider)
					{
						TemplateField actionField = new TemplateField();
						actionField.ItemTemplate = new ToggleNestedTemplate(null);
                        _gridSalesTxn.Grid.Columns.Insert(0, actionField);
					}
                    _gridSalesTxn.Grid.RowDataBound += new GridViewRowEventHandler(Grid_RowDataBound);
                    _gridSalesTxn.Grid.PageIndexChanging += delegate { hdnExpandedRows.Value = string.Empty; };
                    phAcActivityTransactions.Controls.Add(_gridSalesTxn);
                    #endregion

                    #region Create Orphans Grid
                    if (_config.ShowOrphanGrid)
                    {
                        _gridOrphans = new AspDynamicGrid() { AutoRebind = false, EnableViewState = false, CreateTopPanel = false };
						_gridOrphans.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
						_gridOrphans.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
						_gridOrphans.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
                        _gridOrphans.Provider = _orphansGridProvider;
                        phOrphanTransactions.Controls.Add(_gridOrphans);
                    }
                    #endregion

                    if (!IsPostBack)
					{
						PopulateDateControls(_config.DateDisplayType, _config.MinimumDateRange, _config.DatesToDisplay);
                        lnkPrintGrid.Visible = _config.EnablePrinting;                        
					}

                    _gridSalesTxn.Rebind();
                    if (_config.ShowOrphanGrid)
                    {
                        _gridOrphans.Rebind();
                    }
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex);
				throw;
			}
		}
        
		/// <summary>
		/// Page load event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			ddlDateRange.SelectedIndexChanged += new EventHandler(ddlDateRange_SelectedIndexChanged);
			btnSearch.Click += new EventHandler(btnSearch_Click);
			btnSearch.Text = ResourceUtils.GetLocalWebResource(_modulePath, "btnSearch.Text");

			IpcManager.RegisterEventHandler("MemberSelected", this, false);
			IpcManager.RegisterEventHandler("MemberUpdated", this, false);

            lnkPrintGrid.Click += new EventHandler(lnkPrintGrid_Click);

            if (!_memberSelected || _salesTxnGridProvider == null)
			{
				return;
			}
            if (!IsAjaxPostback())
            {
                RebindGrid();
            }
            dpFromDate.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            dpFromDate.Calendar.FastNavigationSettings.CancelButtonCaption = Resources.GlobalResources.CalendarCancelButtonText;
            dpFromDate.Calendar.FastNavigationSettings.OkButtonCaption = Resources.GlobalResources.CalendarOKButtonText;
            dpFromDate.Calendar.FastNavigationSettings.TodayButtonCaption = Resources.GlobalResources.CalendarTodayButtonText;

            dpToDate.Culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            dpToDate.Calendar.FastNavigationSettings.CancelButtonCaption = Resources.GlobalResources.CalendarCancelButtonText;
            dpToDate.Calendar.FastNavigationSettings.OkButtonCaption = Resources.GlobalResources.CalendarOKButtonText;
            dpToDate.Calendar.FastNavigationSettings.TodayButtonCaption = Resources.GlobalResources.CalendarTodayButtonText;


        }

        private string GetTempFilename(string extension)
        {
            string tempDir = IOUtils.GetTempDirectory();
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            tempDir = IOUtils.AppendSeparatorToFolderPath(tempDir);

            string tempFile = string.Format("{0}AccountActivity_{1}.{2}", tempDir, DateTimeUtil.ConvertDateToString("MM-dd-yyyy hh-mm-ss-fffffff", DateTime.Now), extension);
            if (!File.Exists(tempFile))
            {
                FileStream fs = null;
                try
                {
                    fs = File.Create(tempFile);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }
            return tempFile;
        }

        void lnkPrintGrid_Click(object sender, EventArgs e)
        {
            string methodName = "lnkPrintGrid_Click";

            string tempFile = string.Empty;
            try
            {
                tempFile = GetTempFilename("pdf");                
                string filename = Path.GetFileName(tempFile);
                bool doPrinting = false;

                string docTitle = "Account Activity";
                if (!string.IsNullOrEmpty(_config.TitleResourceKey))
                {
                    docTitle = ResourceUtils.GetLocalWebResource(_modulePath, _config.TitleResourceKey, docTitle);
                }
                IList<string> documentTitle = new List<string>();
                documentTitle.Add(Brierley.FrameWork.Common.FrameworkAssemblyVersionInfo.AssemblyProduct);
                documentTitle.Add(docTitle);

                PdfPageHeader pageHeader = new PdfPageHeader();
                pageHeader.HeaderContent = new List<string>();

                string pageTitle = "Sales Transaction";
                if (!string.IsNullOrEmpty(_config.SalesTxnLabelResourceKey))
                {
                    pageTitle = ResourceUtils.GetLocalWebResource(_modulePath, _config.SalesTxnLabelResourceKey, pageTitle);
                }

                pageHeader.HeaderContent.Add(pageTitle);

                pageHeader.HeaderFontAlignment = PdfDocument.PdfAlignment.Center;
                pageHeader.HeaderFontFamily = PdfDocument.PdfFontFamily.Helvetica;
                pageHeader.HeaderFontSize = 20;
                pageHeader.HeaderFontStyle = PdfDocument.PdfFontStyle.Italic;

                using (PdfDocument doc = new PdfDocument(tempFile, pageHeader))
                {
                    doc.Initialize(documentTitle);
                    PortalState.PutInCache("AccountActivityPdf", doc);
                    if (_salesTxnGridProvider.GeneratePdfFromData(tempFile))
                    {
                        doPrinting = true;
                        if (_config.ShowOrphanGrid)
                        {
                            doPrinting = _orphansGridProvider.GeneratePdfFromData(tempFile);
                        }
                    }
                }

                if (doPrinting)
                {
                    using (BinaryReader reader = new BinaryReader(File.OpenRead(tempFile)))
                    {
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.ContentType = "Application/x-pdf";
                        HttpContext.Current.Response.AppendHeader("content-disposition", "attachment; filename=" + filename);

                        byte[] buf = new byte[2048];
                        int nread = -1;
                        while (true)
                        {
                            nread = reader.Read(buf, 0, buf.Length);
                            if (nread <= 0) break;
                            HttpContext.Current.Response.OutputStream.Write(buf, 0, nread);
                        }
                        HttpContext.Current.Response.OutputStream.Flush();
                        HttpContext.Current.Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
            }
            finally
            {
                if (!string.IsNullOrEmpty(tempFile) && File.Exists(tempFile))
                {
                    try
                    {
                        File.Delete(tempFile);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(_className, methodName, string.Format("Can't delete temp file '{0}': {1}", tempFile, ex.Message), ex);
                    }
                }
            }
        }

		protected override void OnPreRender(EventArgs e)
		{
            if (_memberSelected && _salesTxnGridProvider != null)
			{
                lblNoResults.Visible = _salesTxnGridProvider != null && _salesTxnGridProvider.GetNumberOfRows() < 1;
			}
            if (_config.ShowOrphanGrid && _orphansGridProvider != null)
            {
                lblNoPointResults.Visible = _orphansGridProvider != null && _orphansGridProvider.GetNumberOfRows() < 1;
            }
			base.OnPreRender(e);
		}


		protected void btnSearch_Click(object sender, EventArgs e)
		{
			hdnExpandedRows.Value = string.Empty;
		}

		void ddlDateRange_SelectedIndexChanged(object sender, EventArgs e)
		{
			hdnExpandedRows.Value = string.Empty;
		}



		private bool IsAjaxPostback()
		{
			return Page.Request.Form["keyVal"] != null && Page.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
		}

        #region Stuff For Nested Grid
        private DynamicGridColumnSpec GetKeyColumn()
		{
            if (_gridSalesTxn.Columns != null)
			{
                foreach (DynamicGridColumnSpec column in _gridSalesTxn.Columns)
				{
					if (column.IsKey)
					{
						return column;
					}
				}
			}
			return null;
		}
				

		protected void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				IDynamicGridProvider provider = null;
				GridView gridView = null;

                if (sender == _gridSalesTxn.Grid)
				{
                    provider = _salesTxnGridProvider;
                    gridView = _gridSalesTxn.Grid;
                    if (!_nestedGrids.ContainsKey(_gridSalesTxn.Grid))
					{
                        _nestedGrids.Add(_gridSalesTxn.Grid, _salesTxnGridProvider);
					}
				}
				else
				{
					foreach (var gv in _nestedGrids.Keys)
					{
						if (gv == sender)
						{
							provider = _nestedGrids[gv];
							gridView = gv;
							break;
						}
					}
				}
				if (provider is INestedGridProvider && e.Row.RowType == DataControlRowType.DataRow)
				{
					string keyCol = GetKeyColumn().Name;
					int rowIndex = e.Row.DataItemIndex;

					object keyVal = ((DataRowView)e.Row.DataItem).DataView.Table.DefaultView[rowIndex][keyCol];

					var lnkToggleNested = (System.Web.UI.HtmlControls.HtmlAnchor)e.Row.FindControl("lnkToggleNested");
					if (((INestedGridProvider)provider).HasChildren(keyVal))
					{
						lnkToggleNested.Attributes.Add("onclick", "return ToggleNested(this, '" + keyVal + "');");
						lnkToggleNested.HRef = "#";
					}
					else
					{
						lnkToggleNested.Visible = false;
					}

					IDynamicGridProvider p = ((INestedGridProvider)provider).GetChildGridProvider(keyVal);
					if (p != null)
					{
						GridViewRow row = new GridViewRow(-1, -1, DataControlRowType.DataRow, DataControlRowState.Normal);
						row.Attributes.Add("style", "display:none;");
						var td = new TableCell() { ColumnSpan = gridView.Columns.Count };
						row.Cells.Add(td);

						//If preferred method is to not use ajax, grids can be nested as the rows are bound.
						//There is a performance hit when creating the entire grid at bind time, but this
						//may be necessary (ajax implementation only supports one level of nesting)
						if (((INestedGridProvider)provider).NestingType == NestingTypes.DataBound)
						{
							AspDynamicGrid grdChild = new AspDynamicGrid() { CreateTopPanel = false, EnableViewState = false };
							grdChild.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
							grdChild.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
							grdChild.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
							grdChild.Provider = p;
							grdChild.Grid.RowDataBound += new GridViewRowEventHandler(Grid_RowDataBound);
							if (p != null && p is INestedGridProvider)
							{
								TemplateField actionField = new TemplateField();
								actionField.ItemTemplate = new ToggleNestedTemplate(null);
								grdChild.Grid.Columns.Insert(0, actionField);
							}
							_nestedGrids.Add(grdChild.Grid, p);
							td.Controls.Add(grdChild);
						}
						Table tbl = (e.Row.Parent as Table);
						tbl.Rows.Add(row);
					}
					else
					{
						lnkToggleNested.Visible = false;
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, "Grid_RowDataBound", ex.Message, ex);
				throw;
			}
		}
        #endregion

        #region Date Related
        private void PopulateDateControls(DateDisplayTypes displayType, DateTime? minimumDateRange, int? datesToDisplay)
		{
			if (_config.DateDisplayType == DateDisplayTypes.None)
			{
				pnlDateTextBox.Visible = false;
				pnlDateRange.Visible = false;
			}
			else if (_config.DateDisplayType == DateDisplayTypes.TextBoxRange)
			{
				pnlDateTextBox.Visible = true;
				pnlDateRange.Visible = false;
                //var startdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                //var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                var startdate = _config.MinimumDateRange.GetValueOrDefault(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                var endDate = _config.MaximumDateRange.GetValueOrDefault(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)));                
				dpFromDate.SelectedDate = startdate;
				dpToDate.SelectedDate = endDate;
                _gridSalesTxn.SetSearchParm("FromDate", dpFromDate.SelectedDate);
                _gridSalesTxn.SetSearchParm("ToDate", dpToDate.SelectedDate.Value);
                if (_config.ShowOrphanGrid)
                {
                    _gridOrphans.SetSearchParm("FromDate", dpFromDate.SelectedDate);
                    _gridOrphans.SetSearchParm("ToDate", dpToDate.SelectedDate.Value);
                }
			}
			else
			{
				pnlDateTextBox.Visible = false;
				pnlDateRange.Visible = true;

				List<KeyValuePair<DateTime, DateTime>> dates = null;
				switch (_config.DateDisplayType)
				{
					case DateDisplayTypes.DropDownByWeek:
						dates = GetWeeks(minimumDateRange, datesToDisplay);
						break;
					case DateDisplayTypes.DropDownByMonth:
						dates = GetMonths(minimumDateRange, datesToDisplay);
						break;
					case DateDisplayTypes.DropDownByQuarter:
						dates = GetQuarters(minimumDateRange, datesToDisplay);
						break;
					case DateDisplayTypes.DropDownByYear:
						dates = GetYears(minimumDateRange, datesToDisplay);
						break;
				}


				foreach (var range in dates)
				{
					ListItem li = new ListItem();
					li.Value = range.Key.ToShortDateString() + "|" + range.Value.ToShortDateString();
					switch (displayType)
					{
						case DateDisplayTypes.DropDownByWeek:
						case DateDisplayTypes.DropDownByQuarter:
							li.Text = range.Key.ToShortDateString() + " To " + range.Value.ToShortDateString();
							break;
						case DateDisplayTypes.DropDownByYear:
							li.Text = range.Key.Year.ToString();
							break;
						case DateDisplayTypes.DropDownByMonth:
							li.Text = range.Key.ToString("MMMM yyyy");
							break;
					}
					
					ddlDateRange.Items.Add(li);
				}

				var startdate = dates[0].Key;
				var endDate = dates[0].Value;
                _gridSalesTxn.SetSearchParm("FromDate", startdate);
                _gridSalesTxn.SetSearchParm("ToDate", endDate);
                if (_config.ShowOrphanGrid)
                {
                    _gridOrphans.SetSearchParm("FromDate", startdate);
                    _gridOrphans.SetSearchParm("ToDate", endDate);
                }

			}

		}


		private List<KeyValuePair<DateTime, DateTime>> GetWeeks(DateTime? minDate, int? datesToDisplay)
		{
			var ret = new List<KeyValuePair<DateTime, DateTime>>();
			
			DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
			DateTime start = DateTime.Today;
			while (start.DayOfWeek != firstDayOfWeek)
			{
				start = start.AddDays(-1);
			}

			while (start >= minDate.GetValueOrDefault(DateTimeUtil.MinValue) && ret.Count < datesToDisplay.GetValueOrDefault(_maxDateRanges))
			{
				ret.Add(new KeyValuePair<DateTime,DateTime>(start, start.AddDays(6)));
				start = start.AddDays(-7);
			}
			return ret;
		}

		private List<KeyValuePair<DateTime, DateTime>> GetQuarters(DateTime? minDate, int? datesToDisplay)
		{
			var ret = new List<KeyValuePair<DateTime, DateTime>>();
			int quarterStartMonth = 0;
			switch (DateTime.Today.Month)
			{
				case 1: case 2: case 3:
					quarterStartMonth = 1;
					break;
				case 4: case 5: case 6:
					quarterStartMonth = 4;
					break;
				case 7: case 8: case 9:
					quarterStartMonth = 7;
					break;
				case 10: case 11: case 12:
					quarterStartMonth = 10;
					break;
			}
			DateTime start = new DateTime(DateTime.Today.Year, quarterStartMonth, 1);

			while (start >= minDate.GetValueOrDefault(DateTimeUtil.MinValue) && ret.Count < datesToDisplay.GetValueOrDefault(_maxDateRanges))
			{
				DateTime end = start.AddMonths(3).AddDays(-1);
				ret.Add(new KeyValuePair<DateTime,DateTime>(start, end));
				start = start.AddMonths(-3);
			}
			return ret;

		}

		private List<KeyValuePair<DateTime, DateTime>> GetMonths(DateTime? minDate, int? datesToDisplay)
		{
			var ret = new List<KeyValuePair<DateTime, DateTime>>();

			DateTime start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

			while (start >= minDate.GetValueOrDefault(DateTimeUtil.MinValue) && ret.Count < datesToDisplay.GetValueOrDefault(_maxDateRanges))
			{
				DateTime end = start.AddMonths(1).AddDays(-1);
				ret.Add(new KeyValuePair<DateTime,DateTime>(start, end));
				start = start.AddMonths(-1);
			} 
			return ret;

		}

		private List<KeyValuePair<DateTime, DateTime>> GetYears(DateTime? minDate, int? datesToDisplay)
		{
			var ret = new List<KeyValuePair<DateTime, DateTime>>();

			DateTime start = new DateTime(DateTime.Today.Year, 1, 1);

			while (start >= minDate.GetValueOrDefault(DateTimeUtil.MinValue) && ret.Count < datesToDisplay.GetValueOrDefault(_maxDateRanges))
			{
				DateTime end = new DateTime(start.Year, 12, 31);
				ret.Add(new KeyValuePair<DateTime,DateTime>(start, end));
				start = start.AddYears(-1);
			}
			return ret;
		}
        #endregion


        private void RebindGrid()
		{
			switch (_config.DateDisplayType)
			{
				case DateDisplayTypes.None:
					break;
				case DateDisplayTypes.TextBoxRange:
                    _gridSalesTxn.SetSearchParm("FromDate", dpFromDate.SelectedDate);                    
                    _gridSalesTxn.SetSearchParm("ToDate", dpToDate.SelectedDate);
                    if (_config.ShowOrphanGrid)
                    {
                        _gridOrphans.SetSearchParm("FromDate", dpFromDate.SelectedDate);
                        _gridOrphans.SetSearchParm("ToDate", dpToDate.SelectedDate);
                    }
					break;
				case DateDisplayTypes.DropDownByMonth:
				case DateDisplayTypes.DropDownByQuarter:
				case DateDisplayTypes.DropDownByWeek:
				case DateDisplayTypes.DropDownByYear:
					DateTime start = DateTime.Today;
					DateTime end = DateTime.Today;
					if (!string.IsNullOrEmpty(ddlDateRange.SelectedValue))
					{
						var split = ddlDateRange.SelectedValue.Split('|');
						if (split.Length == 2 && DateTime.TryParse(split[0], out start) && DateTime.TryParse(split[1], out end))
						{
                            _gridSalesTxn.SetSearchParm("FromDate", start);
                            _gridSalesTxn.SetSearchParm("ToDate", end);
                            if (_config.ShowOrphanGrid)
                            {
                                _gridOrphans.SetSearchParm("FromDate", start);
                                _gridOrphans.SetSearchParm("ToDate", end);
                            }
						}
					}
					break;
			}

            _gridSalesTxn.Rebind();            
            lblNoResults.Visible = _salesTxnGridProvider != null && _salesTxnGridProvider.GetNumberOfRows() < 1;

            if (_config.ShowOrphanGrid)
            {
                _gridOrphans.Rebind();
                lblNoPointResults.Visible = _orphansGridProvider != null && _orphansGridProvider.GetNumberOfRows() < 1;
            }
		}

		public ModuleConfigurationKey GetConfigurationKey()
		{
			return ConfigurationKey;
		}

		public void HandleEvent(IpcEventInfo info)
		{
			const string methodName = "HandleEvent";
			_logger.Trace(_className, methodName, "Begin");
			if (
				info.PublishingModule != base.ConfigurationKey &&
				(info.EventName == "MemberSelected" || info.EventName == "MemberUpdated") &&
				_config != null
				)
			{
				RebindGrid();
			}
			_logger.Trace(_className, methodName, "End");
		}
	}
}
