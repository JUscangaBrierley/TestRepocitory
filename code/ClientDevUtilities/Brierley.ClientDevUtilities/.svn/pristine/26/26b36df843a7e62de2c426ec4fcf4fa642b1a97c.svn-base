using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_TextBlock")]
	public class TextBlock : ContentDefBase
	{
		public const string TEXTBLOCKNAME = "TextBlock";

		/// <summary>
		/// Gets or sets the Name for the current TextBlock
		/// </summary>
        [PetaPoco.Column(Length = 30, IsNullable = false)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Description for the current TextBlock
		/// </summary>
        [PetaPoco.Column(Length = 30)]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the Version for the current TextBlock
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public int Version { get; set; }

		/// <summary>
		/// Gets or sets the IsLocked for the current TextBlock
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public bool IsLocked { get; set; }

		/// <summary>
		/// Gets or sets the Content for the current TextBlock
		/// </summary>
        [PetaPoco.Column]
		public string Content { get; set; }

		/// <summary>
		/// Initializes a new instance of the TextBlock class
		/// </summary>
		public TextBlock()
			: base(ContentObjType.TextBlock)
		{
			Version = 1;
		}

		public string GetContent(string culture = null, string channel = null)
		{
			if (culture == null)
			{
				culture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
			}
			if (channel == null)
			{
				channel = "Web";
			}

			return GetContent(culture, channel, TEXTBLOCKNAME);
		}

		public void SetContent(string content, string culture = null, string channel = null)
		{
			if (culture == null)
			{
				culture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
			}
			if (channel == null)
			{
				channel = "Web";
			}
			SetContent(culture, channel, TEXTBLOCKNAME, content);
		}

		public bool ConvertTextBlockXML()
		{
			// If we're dealing with an unconverted TextBlock with Content
			if (!string.IsNullOrEmpty(Content) && (Contents == null || Contents.Count == 0))
			{
				// Check that we're dealing with an XML textblock
				if (Content.StartsWith("<textblock>"))
				{
					XElement root = XElement.Parse(Content);
					// Ensure that the XML parsed correctly
					if (root != null) 
					{
						IEnumerable<XElement> languageNodes = root.Elements("culture");
						if (languageNodes != null && languageNodes.Count<XElement>() > 0)
						{
							foreach (XElement languageNode in languageNodes)
							{
								IEnumerable<XElement> channelNodes = languageNode.Elements("channel");
								if (channelNodes != null && channelNodes.Count<XElement>() > 0)
								{
									foreach (XElement channelNode in channelNodes)
									{
										if (!string.IsNullOrEmpty(channelNode.Value))
										{
											string languageName = languageNode.Attribute("name").Value;
											string channelName = channelNode.Attribute("name").Value;
											string value = CryptoUtil.DecodeUTF8(channelNode.Value);
											SetContent(languageName, channelName, TEXTBLOCKNAME, value);
										}
									}
								}
							}
						}
						Content = string.Empty; // Clear the content when finished
						return true;
					}
				}
			}
			return false;
		}

		public TextBlock Clone()
		{
			return Clone(new TextBlock());
		}

		public TextBlock Clone(TextBlock other)
		{
			other.Name = Name;
			other.Description = Description;
			other.Version = Version;
			other.Content = Content;
			other.IsLocked = false;
			return (TextBlock)base.Clone(other);
		}
	}
}
