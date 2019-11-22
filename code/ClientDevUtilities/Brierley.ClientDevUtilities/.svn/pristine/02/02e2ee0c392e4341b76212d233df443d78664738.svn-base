using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Data
{
	public class ContentService : ServiceBase
	{
		private const string _className = "ContentService";
		private const string BATCHID_GENERATOR_NAME = "BatchID";

		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		private RewardDao _rewardDao;
		private LangChanContentDao _langChanContentDao;
		private ContentAttributeDefDao _contentAttributeDefDao;
		private ContentAttributeDao _contentAttributeDao;
		private ProductDao _productDao;
		private ProductVariantDao _productVariantDao;
		private ProductImageDao _productImageDao;
		private CertificateDao _certificateDao;
		private UserAgentMapDao _userAgentMapDao;
		private BatchDao _batchDao;
		private StructuredContentAttributeDao _structuredContentAttributeDao;
		private StructuredContentElementDao _structuredContentElementDao;
		private StructuredContentDataDao _structuredContentDataDao;
		private TemplateDao _templateDao;
		private DocumentDao _documentDao;
		private TextBlockDao _textBlockDao;
		private CategoryDao _categoryDao;
		private LanguageDao _languageDao;
		private ChannelDao _channelDao;
		private StoreDao _storeDao;
		private PromotionDao _promotionDao;
		private BonusDao _bonusDao;
		private CouponDao _couponDao;
		private MessageDao _messageDao;
        private NotificationDao _notificationDao;
        private NotificationCategoryDao _notificationCategoryDao;
        private FulfillmentProviderProductMapDao _fulfillmentProviderProductMapDao;
        private ExchangeRateDao _exchangeRateDao;

		public bool KeepCacheLoaded { get; set; }

		public RewardDao RewardDao
		{
			get
			{
				if (_rewardDao == null)
				{
					_rewardDao = new RewardDao(Database, Config, LangChanContentDao, ContentAttributeDao);
				}
				return _rewardDao;
			}
		}

		public LangChanContentDao LangChanContentDao
		{
			get
			{
				if (_langChanContentDao == null)
				{
					_langChanContentDao = new LangChanContentDao(Database, Config);
				}
				return _langChanContentDao;
			}
		}

		public ContentAttributeDefDao ContentAttributeDefDao
		{
			get
			{
				if (_contentAttributeDefDao == null)
				{
					_contentAttributeDefDao = new ContentAttributeDefDao(Database, Config);
				}
				return _contentAttributeDefDao;
			}
		}

		public ContentAttributeDao ContentAttributeDao
		{
			get
			{
				if (_contentAttributeDao == null)
				{
					_contentAttributeDao = new ContentAttributeDao(Database, Config);
				}
				return _contentAttributeDao;
			}
		}

		public ProductDao ProductDao
		{
			get
			{
				if (_productDao == null)
				{
					_productDao = new ProductDao(Database, Config, LangChanContentDao, ContentAttributeDao);
				}
				return _productDao;
			}
		}

		public ProductVariantDao ProductVariantDao
		{
			get
			{
				if (_productVariantDao == null)
				{
					_productVariantDao = new ProductVariantDao(Database, Config);
				}
				return _productVariantDao;
			}
		}

		public ProductImageDao ProductImageDao
		{
			get
			{
				if (_productImageDao == null)
				{
					_productImageDao = new ProductImageDao(Database, Config);
				}
				return _productImageDao;
			}
		}

		public CertificateDao CertificateDao
		{
			get
			{
				if (_certificateDao == null)
				{
					_certificateDao = new CertificateDao(Database, Config);
				}
				return _certificateDao;
			}
		}

		public UserAgentMapDao UserAgentMapDao
		{
			get
			{
				if (_userAgentMapDao == null)
				{
					_userAgentMapDao = new UserAgentMapDao(Database, Config);
				}
				return _userAgentMapDao;
			}
		}

		public BatchDao BatchDao
		{
			get
			{
				if (_batchDao == null)
				{
					_batchDao = new BatchDao(Database, Config);
				}
				return _batchDao;
			}
		}

		public StructuredContentAttributeDao StructuredContentAttributeDao
		{
			get
			{
				if (_structuredContentAttributeDao == null)
				{
					_structuredContentAttributeDao = new StructuredContentAttributeDao(Database, Config);
				}
				return _structuredContentAttributeDao;
			}
		}

		public StructuredContentElementDao StructuredContentElementDao
		{
			get
			{
				if (_structuredContentElementDao == null)
				{
					_structuredContentElementDao = new StructuredContentElementDao(Database, Config);
				}
				return _structuredContentElementDao;
			}
		}

		public StructuredContentDataDao StructuredContentDataDao
		{
			get
			{
				if (_structuredContentDataDao == null)
				{
					_structuredContentDataDao = new StructuredContentDataDao(Database, Config, StructuredContentAttributeDao);
				}
				return _structuredContentDataDao;
			}
		}

		public TemplateDao TemplateDao
		{
			get
			{
				if (_templateDao == null)
				{
					_templateDao = new TemplateDao(Database, Config);
				}
				return _templateDao;
			}
		}

		public DocumentDao DocumentDao
		{
			get
			{
				if (_documentDao == null)
				{
					_documentDao = new DocumentDao(Database, Config);
				}
				return _documentDao;
			}
		}

		public TextBlockDao TextBlockDao
		{
			get
			{
				if (_textBlockDao == null)
				{
					_textBlockDao = new TextBlockDao(Database, Config, LangChanContentDao, ContentAttributeDao);
				}
				return _textBlockDao;
			}
		}

		public CategoryDao CategoryDao
		{
			get
			{
				if (_categoryDao == null)
				{
					_categoryDao = new CategoryDao(Database, Config);
				}
				return _categoryDao;
			}
		}

		public LanguageDao LanguageDao
		{
			get
			{
				if (_languageDao == null)
				{
					_languageDao = new LanguageDao(Database, Config);
				}
				return _languageDao;
			}
		}

		public ChannelDao ChannelDao
		{
			get
			{
				if (_channelDao == null)
				{
					_channelDao = new ChannelDao(Database, Config);
				}
				return _channelDao;
			}
		}

		public StoreDao StoreDao
		{
			get
			{
				if (_storeDao == null)
				{
					_storeDao = new StoreDao(Database, Config);
				}
				return _storeDao;
			}
		}

		public PromotionDao PromotionDao
		{
			get
			{
				if (_promotionDao == null)
				{
					_promotionDao = new PromotionDao(Database, Config, LangChanContentDao, ContentAttributeDao);
				}
				return _promotionDao;
			}
		}

		public BonusDao BonusDao
		{
			get
			{
				if (_bonusDao == null)
				{
					_bonusDao = new BonusDao(Database, Config, LangChanContentDao, ContentAttributeDao);
				}
				return _bonusDao;
			}
		}

		public CouponDao CouponDao
		{
			get
			{
				if (_couponDao == null)
				{
					_couponDao = new CouponDao(Database, Config, LangChanContentDao, ContentAttributeDao);
				}
				return _couponDao;
			}
		}

		public MessageDao MessageDao
		{
			get
			{
				if (_messageDao == null)
				{
					_messageDao = new MessageDao(Database, Config, LangChanContentDao, ContentAttributeDao);
				}
				return _messageDao;
			}
		}

        public NotificationDao NotificationDao
        {
            get
            {
                if (_notificationDao == null)
                {
                    _notificationDao = new NotificationDao(Database, Config, LangChanContentDao, ContentAttributeDao);
                }
                return _notificationDao;
            }
        }

        public NotificationCategoryDao NotificationCategoryDao
        {
            get
            {
                if (_notificationCategoryDao == null)
                {
                    _notificationCategoryDao = new NotificationCategoryDao(Database, Config);
                }
                return _notificationCategoryDao;
            }
        }

        public FulfillmentProviderProductMapDao FulfillmentProviderProductMapDao
		{
			get
			{
				if (_fulfillmentProviderProductMapDao == null)
				{
					_fulfillmentProviderProductMapDao = new FulfillmentProviderProductMapDao(Database, Config);
				}
				return _fulfillmentProviderProductMapDao;
			}
		}

        public ExchangeRateDao ExchangeRateDao
        {
            get
            {
                if(_exchangeRateDao == null)
                {
                    _exchangeRateDao = new ExchangeRateDao(Database, Config);
                }
                return _exchangeRateDao;
            }
        }

		public ContentService(ServiceConfig config)
			: base(config)
		{
		}

		public ContentProperties GetContentProperties()
		{
			return new ContentProperties(Organization, Environment);
		}

		#region Cache Management

		public void SetKeepCacheLoaded(bool value)
		{
			KeepCacheLoaded = value;
		}


		public void LoadCache(bool keepCacheLoaded)
		{
			KeepCacheLoaded = keepCacheLoaded;
			InitializeDocuments();
			InitializeTextBlocks();
			InitializeStructuredContentMeta();
		}

		public void InitializeDocuments()
		{
			List<Template> templates = TemplateDao.RetrieveAll();
			if (templates != null)
			{
				foreach (Template template in templates)
				{
					CacheManager.Update(CacheRegions.TemplateById, template.ID, template);
				}
			}
			List<Document> docs = DocumentDao.RetrieveAll();
			if (docs != null)
			{
				foreach (Document doc in docs)
				{
					CacheManager.Update(CacheRegions.DocumentById, doc.ID, doc);
				}
			}
		}

		public void InitializeTextBlocks()
		{
			List<TextBlock> textBlocks = GetTextBlocks();
			if (textBlocks != null)
			{
				foreach (TextBlock textblock in textBlocks)
				{
					CacheManager.Update(CacheRegions.TextBlockById, textblock.Id, textblock);
				}
			}
		}

		public void InitializeStructuredContentMeta()
		{
			string methodName = "InitializeStructuredContentMeta";

			_logger.Trace(_className, methodName, "Initializing structured content meta data.");
			// Retrieve all structured content elements
			List<long> elementIds = new List<long>();
			Dictionary<long, StructuredContentElement> elementMap = new Dictionary<long, StructuredContentElement>();
			List<StructuredContentElement> elements = StructuredContentElementDao.RetrieveAll();
			if (elements != null)
			{
				_logger.Debug(_className, methodName, "Caching " + elements.Count + " structured content elements.");
				foreach (StructuredContentElement element in elements)
				{
					CacheManager.Update(CacheRegions.ContentElementByName, element.Name, element);
					CacheManager.Update(CacheRegions.ContentElementById, element.ID, element);
					elementIds.Add(element.ID);
					elementMap.Add(element.ID, element);
				}
				// retrieve all their content attributes
				List<StructuredContentAttribute> attsList = StructuredContentAttributeDao.RetrieveByElementIds(elementIds.ToArray());
				if (attsList != null)
				{
					_logger.Debug(_className, methodName, "Caching " + attsList.Count + " structure content attributes.");
					foreach (StructuredContentAttribute att in attsList)
					{
						StructuredContentElement element = elementMap[att.ElementID];
						if (element.Attributes == null)
						{
							element.Attributes = new List<StructuredContentAttribute>();
						}
						element.Attributes.Add(att);
						CacheManager.Update(CacheRegions.ContentAttributeById, att.ID, att);
					}
				}
			}
			// retrieve all global attributes
			List<StructuredContentAttribute> globalAtts = StructuredContentAttributeDao.RetrieveAllGlobals();
			if (globalAtts != null)
			{
				CacheManager.Update(CacheRegions.GlobalAttributes, "list", globalAtts);
				_logger.Debug(_className, methodName, "Caching " + globalAtts.Count + " global structure content attributes.");
				foreach (StructuredContentAttribute att in globalAtts)
				{
					CacheManager.Update(CacheRegions.GlobalAttributeByName, att.Name, att);
					CacheManager.Update(CacheRegions.ContentAttributeById, att.ID, att);
				}
			}
			else
			{
				CacheManager.Update(CacheRegions.GlobalAttributes, "list", new List<StructuredContentAttribute>());
			}
		}

		#endregion

		#region Product Management

		#region Product Definition

		public void CreateProduct(Product product)
		{
			if (GetProduct(product.Name) != null)
			{
				throw new LWException("Product name already exists.  Please provide a different product name.");
			}
			if (!DuplicateProductPartNumberAllowed())
			{
				Product existing = GetProductByPartNumber(product.PartNumber);
				if (!string.IsNullOrEmpty(product.PartNumber) && existing != null)
				{
					throw new LWException("Product part number already exists.  Please provide a different product part number.");
				}
			}
			_logger.Trace(_className, "CreateProduct", "Creating new product " + product.Name);
			ProductDao.Create(product);
			UpdateProductsCache(product);
		}

		public void CreateProducts(List<Product> products)
		{
			ProductDao.Create(products);
		}

		public void UpdateProduct(Product product, bool deep = true)
		{
			if (!DuplicateProductPartNumberAllowed() && !string.IsNullOrEmpty(product.PartNumber))
			{
				long existing = ProductDao.RetrieveIdByPartNumber(product.PartNumber);
				if (existing > 0 && existing != product.Id)
				{
					// there is an existing product
					throw new LWException("Product part number already exists.  Please provide a different product part number.");
				}
			}
			ProductDao.Update(product, deep);
			UpdateProductsCache(product);
		}

		public void UpdateProducts(List<Product> products, bool deep)
		{
			ProductDao.Update(products, deep);
		}

        public long UpdateProductQuantity(long id, long changeBy)
        {
            CacheManager.Remove(CacheRegions.ProductById, id);
            return ProductDao.UpdateQuantity(id, changeBy);
        }

		public Product GetProduct(long productId)
		{
			Product product = CacheManager.Get(CacheRegions.ProductById, productId) as Product;
			if (product == null)
			{
				product = ProductDao.Retrieve(productId);
				if (product != null)
				{
					CacheManager.Update(CacheRegions.ProductById, product.Id, product);
				}
			}
			return product;
		}

		public Product GetProduct(string name)
		{
			return ProductDao.Retrieve(name);
		}

		public Product GetProductByPartNumber(string partNumber)
		{
			return ProductDao.RetrieveByPartNumber(partNumber);
		}

		public Product GetProductByVariantPartNumber(string partNumber)
		{
			return ProductDao.RetrieveByVariantPartNumber(partNumber);
		}

		public long HowManyProducts()
		{
			return ProductDao.HowMany();
		}

		public List<Product> GetProductsByCategory(long categoryId, bool visibleInLN)
		{
			return ProductDao.RetrieveByCategory(categoryId, visibleInLN);
		}

		public List<Product> GetProductsByCategorySortedByName(long categoryId, bool visibleInLN)
		{
			return ProductDao.RetrieveByCategorySortedByName(categoryId, visibleInLN) ?? new List<Product>();
		}

		public List<Product> GetAllProducts(bool visibleInLN)
		{
			//todo: this seems like it should be checking the cache and returning all products from the cache, if it exists. The method, 
			//before converting to PetaPoco was not checking the cache, so it has been left that way for now. 
			List<Product> list = ProductDao.RetrieveAll(visibleInLN) ?? new List<Product>();
			CacheManager.Update(CacheRegions.Products, CacheRegions.Products, list);
			return list;
		}

		public List<Product> GetAllProducts(long[] ids, bool retrieveContent)
		{
			return ProductDao.RetrieveAll(ids, retrieveContent) ?? new List<Product>();
		}

		public List<Product> GetAllProductsSortedByName(bool visibleInLN)
		{
			return ProductDao.RetrieveAllSortedByName(visibleInLN) ?? new List<Product>();
		}

		public List<Product> GetAllChangedProducts(DateTime since)
		{
			return ProductDao.RetrieveChangedObjects(since) ?? new List<Product>();
		}

		public List<long> GetProductIds(List<Dictionary<string, object>> parms, string sortExpression, bool ascending, bool visibleInLN)
		{
			return ProductDao.RetrieveProductIdsByProperty(parms, sortExpression, ascending, visibleInLN);
		}

		public List<long> GetAllProductIds(string sortExpression, bool ascending, bool visibleInLN)
		{
			return ProductDao.RetrieveAllProductIds(sortExpression, ascending, visibleInLN);
		}

		public List<Product> GetProductsByUserField(long userField)
		{
			return ProductDao.RetrieveByUserField(userField) ?? new List<Product>();
		}

		public List<Product> GetProductsByUserField(string userField)
		{
			return ProductDao.RetrieveByUserField(userField) ?? new List<Product>();
		}

		public void DeleteProduct(long productId)
		{
			_logger.Trace(_className, "DeleteProduct", "Deleting product with id = " + productId);
			ProductDao.Delete(productId);
			InvalidateProductsCache();
			CacheManager.Remove(CacheRegions.ProductById, productId);
		}


		#endregion

		#region Product Variants Management


		public void CreateProductVariant(ProductVariant variant)
		{
			if (GetProductVariantByProductAndDescription(variant.ProductId, variant.VariantDescription) != null)
			{
				throw new LWException("Product variant description already exists. Please provide a different product variant description.");
			}
			if (!string.IsNullOrEmpty(variant.PartNumber) && GetProductVariantByProductAndPartNumber(variant.ProductId, variant.PartNumber) != null)
			{
				throw new LWException("Product variant part number already exists.  Please provide a different product variant part number.");
			}
			ProductVariantDao.Create(variant);
		}

		public void UpdateProductVariant(ProductVariant variant)
		{
			ProductVariant existing = null;
			if (!string.IsNullOrEmpty(variant.PartNumber))
			{
				existing = GetProductVariantByProductAndDescription(variant.ProductId, variant.VariantDescription);
				if (existing != null && existing.ID != variant.ID)
				{
					throw new LWException("Product variant description already exists.  Please provide a different product variant description.");
				}
				existing = GetProductVariantByProductAndPartNumber(variant.ProductId, variant.PartNumber);
				if (existing != null && existing.ID != variant.ID)
				{
					throw new LWException("Product variant part number already exists.  Please provide a different product variant part number.");
				}
			}
			ProductVariantDao.Update(variant);
		}

        public long UpdateProductVariantQuantity(long id, long changeBy)
        {
            return ProductVariantDao.UpdateQuantity(id, changeBy);
        }

        public ProductVariant GetProductVariant(long id)
		{
			return ProductVariantDao.Retrieve(id);
		}

		public ProductVariant GetProductVariantByProductAndPartNumber(long productId, string partNumber)
		{
			return ProductVariantDao.RetrieveByProductAndPartNumber(productId, partNumber);
		}

		public ProductVariant GetProductVariantByProductAndDescription(long productId, string description)
		{
			return ProductVariantDao.RetrieveByProductAndDescription(productId, description);
		}

		public List<ProductVariant> GetAllProductVariantsByProduct(long productId)
		{
			return ProductVariantDao.RetrieveByProduct(productId) ?? new List<ProductVariant>();
		}

		public List<ProductVariant> GetAllChangedProductVariants(DateTime since)
		{
			return ProductVariantDao.RetrieveChangedObjects(since) ?? new List<ProductVariant>();
		}

		public List<ProductVariant> GetAllProductVariants()
		{
			return ProductVariantDao.RetrieveAll() ?? new List<ProductVariant>();
		}

		public void DeleteProductVariant(long imageId)
		{
			ProductVariantDao.Delete(imageId);
		}


		#endregion

		#region Product Map
		public void CreateFulfillmentProviderProductMap(FulfillmentProviderProductMap map)
		{
			FulfillmentProviderProductMapDao.Create(map);
			if (map.ProductId != null)
			{
				CacheManager.Update(CacheRegions.FulfillmentProviderProductMapByProduct, (long)map.ProductId, map);
			}
			if (map.ProductVariantId != null)
			{
				CacheManager.Update(CacheRegions.FulfillmentProviderProductMapByVariant, (long)map.ProductVariantId, map);
			}
		}

		public void UpdateFulfillmentProviderProductMap(FulfillmentProviderProductMap map)
		{
			FulfillmentProviderProductMapDao.Update(map);
			if (map.ProductId != null)
			{
				CacheManager.Update(CacheRegions.FulfillmentProviderProductMapByProduct, (long)map.ProductId, map);
			}
			if (map.ProductVariantId != null)
			{
				CacheManager.Update(CacheRegions.FulfillmentProviderProductMapByVariant, (long)map.ProductVariantId, map);
			}
		}

		public FulfillmentProviderProductMap GetFulfillmentProviderProductMapByProduct(long providerId, long productId)
		{
			FulfillmentProviderProductMap map = (FulfillmentProviderProductMap)CacheManager.Get(CacheRegions.FulfillmentProviderProductMapByProduct, productId);
			if (map == null)
			{
				map = FulfillmentProviderProductMapDao.RetrieveByProduct(providerId, productId);
				if (map != null)
				{
					if (map.ProductId != null)
					{
						CacheManager.Update(CacheRegions.FulfillmentProviderProductMapByProduct, (long)map.ProductId, map);
					}
					if (map.ProductVariantId != null)
					{
						CacheManager.Update(CacheRegions.FulfillmentProviderProductMapByVariant, (long)map.ProductVariantId, map);
					}
				}
			}
			return map;
		}

		public FulfillmentProviderProductMap GetFulfillmentProviderProductMapByProductVariant(long providerId, long variantId)
		{
			FulfillmentProviderProductMap map = (FulfillmentProviderProductMap)CacheManager.Get(CacheRegions.FulfillmentProviderProductMapByVariant, variantId);
			if (map == null)
			{
				map = FulfillmentProviderProductMapDao.RetrieveByProductVariant(providerId, variantId);
				if (map != null)
				{
					if (map.ProductId != null)
					{
						CacheManager.Update(CacheRegions.FulfillmentProviderProductMapByProduct, (long)map.ProductId, map);
					}
					if (map.ProductVariantId != null)
					{
						CacheManager.Update(CacheRegions.FulfillmentProviderProductMapByVariant, (long)map.ProductVariantId, map);
					}
				}
			}
			return map;
		}

		public FulfillmentProviderProductMap GetFulfillmentProviderProductMapByProviderPartNumber(long providerId, string partNumber)
		{
			FulfillmentProviderProductMap map = FulfillmentProviderProductMapDao.RetrieveByProviderPartNumber(providerId, partNumber);
			if (map != null)
			{
				if (map.ProductId != null)
				{
					CacheManager.Update(CacheRegions.FulfillmentProviderProductMapByProduct, (long)map.ProductId, map);
				}
				if (map.ProductVariantId != null)
				{
					CacheManager.Update(CacheRegions.FulfillmentProviderProductMapByVariant, (long)map.ProductVariantId, map);
				}
			}
			return map;
		}

		public void DeleteFulfillmentProviderProductMap(long id)
		{
			FulfillmentProviderProductMap map = FulfillmentProviderProductMapDao.Retrieve(id);
			if (map != null)
			{
				FulfillmentProviderProductMapDao.Delete(id);
				if (map.ProductId != null)
				{
					CacheManager.Remove(CacheRegions.FulfillmentProviderProductMapByProduct, (long)map.ProductId);
				}
				if (map.ProductVariantId != null)
				{
					CacheManager.Remove(CacheRegions.FulfillmentProviderProductMapByVariant, (long)map.ProductVariantId);
				}
			}
		}
		#endregion

		#region Product Image Management


		public void CreateProductImage(ProductImage image)
		{
			ProductImageDao.Create(image);
		}

		public void UpdateProductImage(ProductImage image)
		{
			ProductImageDao.Update(image);
		}

		public ProductImage GetProductImage(long id)
		{
			return ProductImageDao.Retrieve(id);
		}

		public List<ProductImage> GetAllImagesByProduct(long productId)
		{
			return ProductImageDao.RetrieveByProduct(productId) ?? new List<ProductImage>();
		}

		public List<ProductImage> GetAllChangedProductImages(DateTime since)
		{
			return ProductImageDao.RetrieveChangedObjects(since) ?? new List<ProductImage>();
		}

		public ProductImage GetImageByProduct(long productId, string image)
		{
			return ProductImageDao.RetrieveByProduct(productId, image);
		}

		public List<ProductImage> GetAllProductImages()
		{
			return ProductImageDao.RetrieveAll() ?? new List<ProductImage>();
		}

		public void DeleteProductImage(long imageId)
		{
			ProductImageDao.Delete(imageId);
		}

		#endregion

		#endregion

		#region Reward Definitions

		public void CreateRewardDef(RewardDef reward)
		{
			RewardDao.Create(reward);
			CacheManager.Update(CacheRegions.RewardByName, reward.Name, reward);
		}

		public void UpdateRewardDef(RewardDef reward)
		{
			RewardDao.Update(reward);
			CacheManager.Update(CacheRegions.RewardByName, reward.Name, reward);
		}

		public Category GetRewardDefCategory(long rewardDefId)
		{
			return RewardDao.RetrieveCategoryForRewardDef(rewardDefId);
		}

		public List<RewardDef> GetAllRewardDefs()
		{
            List<RewardDef> list = CacheManager.Get(CacheRegions.Rewards, "all") as List<RewardDef>;
		    if (list == null)
		    {
		        list = RewardDao.RetrieveAll() ?? new List<RewardDef>();
                CacheManager.Update(CacheRegions.Rewards, "all", list);
                foreach (RewardDef reward in list)
		        {
		            CacheManager.Update(CacheRegions.RewardByName, reward.Name, reward);
		        }
		    }
		    return list;
		}

		public List<RewardDef> GetAllRewardDefs(long[] ids)
		{
			List<RewardDef> list = RewardDao.Retrieve(ids) ?? new List<RewardDef>();
			foreach (RewardDef reward in list)
			{
				CacheManager.Update(CacheRegions.RewardByName, reward.Name, reward);
			}
			return list;
		}

		public List<long> GetAllRewardDefIds(string sortExpression, bool ascending)
		{
			return RewardDao.RetrieveAllIds(sortExpression, ascending);
		}

		public List<RewardDef> GetRewardDefsByCategory(long categoryId)
		{
			List<RewardDef> list = RewardDao.RetrieveRewardDefsByCategory(categoryId) ?? new List<RewardDef>();
			foreach (RewardDef reward in list)
			{
				CacheManager.Update(CacheRegions.RewardByName, reward.Name, reward);
			}
			return list;
		}

		public List<RewardDef> GetRewardDefsByTier(long tierId)
		{
			List<RewardDef> list = RewardDao.RetrieveByTier(tierId) ?? new List<RewardDef>();
			foreach (RewardDef reward in list)
			{
				CacheManager.Update(CacheRegions.RewardByName, reward.Name, reward);
			}
			return list;
		}

		public List<RewardDef> GetRewardDefsByCertificateType(string certTypeCode)
		{
			List<RewardDef> rewards = RewardDao.RetrieveByCertificateTypeCode(certTypeCode) ?? new List<RewardDef>();
			foreach (RewardDef reward in rewards)
			{
				CacheManager.Update(CacheRegions.RewardByName, reward.Name, reward);
			}
			return rewards;
		}

		public List<long> GetRewardDefIds(List<Dictionary<string, object>> parms, string sortExpression, bool ascending)
		{
			return RewardDao.RetrieveRewardDefIdsByProperty(parms, sortExpression, ascending);
		}

		public List<RewardDef> GetRewardDefsByProperty(List<Dictionary<string, object>> parms, string sortExpression, bool ascending, LWQueryBatchInfo batchInfo, long? categoryId = null)
		{
			if (batchInfo != null)
			{
				batchInfo.Validate();
			}
			return RewardDao.RetrieveRewardDefsByProperty(parms, sortExpression, ascending, batchInfo, categoryId) ?? new List<RewardDef>();
		}

		public int HowManyRewardDefs(List<Dictionary<string, object>> parms, long? categoryId = null)
		{
			return RewardDao.HowManyRewards(parms, categoryId);
		}

		public List<RewardDef> GetAllChangedRewardDefs(DateTime since)
		{
			return RewardDao.RetrieveChangedObjects(since) ?? new List<RewardDef>();
		}

		public RewardDef GetRewardDef(long rewardId)
		{
			var reward = RewardDao.Retrieve(rewardId);
			if (reward != null)
			{
				CacheManager.Update(CacheRegions.RewardByName, reward.Name, reward);
			}
			return reward;
		}

		public RewardDef GetRewardDef(string rewardName)
		{
			RewardDef reward = (RewardDef)CacheManager.Get(CacheRegions.RewardByName, rewardName);
			if (reward == null)
			{
				reward = RewardDao.Retrieve(rewardName);
				if (reward != null)
				{
					CacheManager.Update(CacheRegions.RewardByName, reward.Name, reward);
				}
			}
			return reward;
		}

        public List<RewardDef> GetRewardDefsByType(RewardType rewardType)
        {
            return RewardDao.RetrieveByRewardType(rewardType) ?? new List<RewardDef>();
        }

        public RewardDef GetRewardDefForExchange(Member member)
        {
            List<RewardDef> rewardDefs = GetRewardDefsByType(RewardType.Payment);
            if (rewardDefs == null || rewardDefs.Count == 0) // If we don't have any payment rewards, we can't select one
                return null;

            RewardDef noTierReward = (from r in rewardDefs where !r.TierId.HasValue select r).FirstOrDefault(); // Get the no tier reward

            if (noTierReward != null)
                return noTierReward;

            MemberTier mt = member.GetTier(DateTime.Now);

            if (mt == null) // Since we don't have a tier and there's no tier-less reward, we can't match a reward for the member
                return null;

            // Otherwise, we'll select the first reward that matches the member's tier or null
            return (from r in rewardDefs where r.TierId.HasValue && r.TierId.Value == mt.TierDefId select r).FirstOrDefault();
        }

		public void DeleteRewardDef(long rewardId)
		{
			RewardDef reward = GetRewardDef(rewardId);
			if (reward != null)
			{
				RewardDao.Delete(rewardId);
				CacheManager.Remove(CacheRegions.RewardByName, reward.Name);
			}
		}

		#endregion

		#region Certificates

		public void CreatePromoCertificate(PromotionCertificate cert)
		{
			CertificateDao.Create(cert);
		}

		public void CreatePromoCertificate(List<PromotionCertificate> certs)
		{
			CertificateDao.Create(certs);
		}

		public void GeneratePromoCertificates(int quantity, string certNumberFormat, string typeCode, ContentObjType contentObjectType, DateTime? startDate, DateTime? endDate)
		{
			const string methodName = "GeneratePromoCertificates";
			if (string.IsNullOrEmpty(certNumberFormat) || !certNumberFormat.Contains('#'))
			{
				throw new LWException("Unable to find any '#'s in the format string.");
			}
			string msg1 = string.Format("Generating {0} '{1}' certs using format '{2}' for typeCode '{3}'", quantity, contentObjectType.ToString(), certNumberFormat, typeCode);
			_logger.Debug(_className, methodName, msg1);

			string outputCertNumberFormat = certNumberFormat;
			List<int> buckets = ParseCertNumberFormat(certNumberFormat, ref outputCertNumberFormat);

			Random random = new Random((int)(DateTime.Now.Ticks % int.MaxValue));

			for (int i = 0; i < quantity; i++)
			{
				PromotionCertificate cert = new PromotionCertificate()
				{
					TypeCode = typeCode,
					ObjectType = contentObjectType,
					Available = true,
					StartDate = startDate,
					ExpiryDate = endDate
				};

				int attempts = 0;
				bool created = false;
				while (!created && attempts < 10)
				{
					cert.CertNmbr = GeneratePromoCertNumber(buckets, random, outputCertNumberFormat);

					try
					{
						CertificateDao.Create(cert);
						created = true;
					}
					catch (Exception ex)
					{
						if (ex.InnerException != null && Regex.IsMatch(ex.InnerException.Message, "duplicate|unique"))
						{
							// ignore exception until # attempts exceeded
							attempts++;
							string msg = string.Format("Collision on cert number '{0}', attempt = {1}", cert.CertNmbr, attempts);
							_logger.Error(_className, methodName, msg);
						}
						else
						{
							throw;
						}
					}
				}
				if (!created)
				{
					string msg = string.Format("Unable to create unique certificate number after {0} attempts", attempts);
					_logger.Error(_className, methodName, msg);
					throw new LWException("Unable to create unique certificate number.  Please ensure format string has enough '#'s");
				}
			}
			_logger.Debug(_className, methodName, "Done");
		}



		public PromotionCertificate RetrieveFirstAvailablePromoCertificate(ContentObjType? objectType, string typeCode, DateTime? startDate, DateTime? endDate)
		{
			return CertificateDao.RetrieveFirstAvailable(objectType, typeCode, startDate, endDate);
		}

		public List<PromotionCertificate> RetrieveAvailablePromoCertificate(ContentObjType? objectType, string typeCode, DateTime? startDate, DateTime? endDate, int howMany)
		{
			return CertificateDao.RetrieveAvailable(objectType, typeCode, startDate, endDate, howMany);
		}

		public long HowManyPromoCertificates(ContentObjType? objectType, string typeCode, DateTime? startDate, DateTime? endDate, bool? available)
		{
			return CertificateDao.HowMany(objectType, typeCode, startDate, endDate, available);
		}

		public int ReclaimCertificates(string[] certNmbrs)
		{
			return CertificateDao.ReclaimCertificates(certNmbrs);
		}

		public void DeletePromoCertificate(string certNmbr)
		{
			CertificateDao.Delete(certNmbr);
		}

		#endregion

		#region User Agent Mapping

		public void CreateUserAgentMap(UserAgentMap map)
		{
			UserAgentMapDao.Create(map);
		}

		public void UpdateUserAgentMap(UserAgentMap map)
		{
			UserAgentMapDao.Update(map);
		}

		public void DeleteUserAgentMap(long id)
		{
			UserAgentMapDao.Delete(id);
		}

		public UserAgentMap GetUserAgentMap(long id)
		{
			return UserAgentMapDao.Retrieve(id);
		}

		public UserAgentMap GetUserAgentMap(string userAgent)
		{
			return UserAgentMapDao.Retrieve(userAgent);
		}

		public List<UserAgentMap> GetAllUserAgentMaps()
		{
			return UserAgentMapDao.RetrieveAll() ?? new List<UserAgentMap>();
		}

		public List<UserAgentMap> GetAllUserAgentMaps(DateTime changedSince)
		{
			return UserAgentMapDao.RetrieveAll(changedSince) ?? new List<UserAgentMap>();
		}

		#endregion

		#region Documents

		public void CreateDocument(Document document)
		{
			DocumentDao.Create(document);
		}

		public void UpdateDocument(Document document)
		{
			DocumentDao.Update(document);
		}

		public void DeleteDocument(long id)
		{
			DocumentDao.Delete(id);
		}

		public Document GetDocument(long id)
		{
			Document doc = (Document)CacheManager.Get(CacheRegions.DocumentById, id);
			if (doc == null)
			{
				if (KeepCacheLoaded)
				{
					InitializeDocuments();
					doc = (Document)CacheManager.Get(CacheRegions.DocumentById, id);
				}
				else
				{
					doc = DocumentDao.Retrieve(id);
				}
			}
			return doc;
		}

		public Document GetDocument(string name)
		{
			return DocumentDao.Retrieve(name);
		}

		public List<Document> GetDocuments()
		{
			return DocumentDao.RetrieveAll() ?? new List<Document>();
		}

		public List<Document> GetDocuments(DocumentType docType)
		{
			return DocumentDao.RetrieveAll(docType) ?? new List<Document>();
		}

		public List<Document> GetDocuments(DocumentType docType, DateTime changedSince)
		{
			return DocumentDao.RetrieveAll(docType, changedSince) ?? new List<Document>();
		}

		public List<Document> GetSurveyRunnerDocuments()
		{
			return DocumentDao.RetrieveSurveyRunnerDocuments() ?? new List<Document>();
		}

		#endregion

		#region Templates

		public void CreateTemplate(Template template)
		{
			TemplateDao.Create(template);
		}

		public void UpdateTemplate(Template template)
		{
			TemplateDao.Update(template);
		}

		public void DeleteTemplate(long id)
		{
			TemplateDao.Delete(id);
		}

		public Template GetTemplate(long id)
		{
			Template template = (Template)CacheManager.Get(CacheRegions.TemplateById, id);
			if (template == null)
			{
				if (KeepCacheLoaded)
				{
					InitializeDocuments();
					template = (Template)CacheManager.Get(CacheRegions.TemplateById, id);
				}
				else
				{
					template = TemplateDao.Retrieve(id);
				}
			}
			return template;
		}

		public Template GetTemplate(string name)
		{
			return TemplateDao.Retrieve(name);
		}

		public List<Template> GetTemplates()
		{
			return TemplateDao.RetrieveAll() ?? new List<Template>();
		}

		public List<Template> GetTemplatesByFolder(long folderId)
		{
			return TemplateDao.RetrieveByFolder(folderId) ?? new List<Template>();
		}

		public List<Template> GetTemplates(TemplateType type)
		{
			return TemplateDao.Retrieve(type) ?? new List<Template>();
		}

		public List<Template> GetChangedTemplates(TemplateType type, DateTime changedSince)
		{
			return TemplateDao.Retrieve(type, changedSince) ?? new List<Template>();
		}

		public List<Template> GetSurveyRunnerTemplates()
		{
			return TemplateDao.RetrieveSurveyRunnerTemplates() ?? new List<Template>();
		}

		#endregion

		#region TextBlock

		public void CreateTextBlock(TextBlock textBlock)
		{
			TextBlockDao.Create(textBlock);
			CacheManager.Update(CacheRegions.TextBlockById, textBlock.Id, textBlock);
		}

		public void UpdateTextBlock(TextBlock textBlock)
		{
			TextBlockDao.Update(textBlock);
			CacheManager.Update(CacheRegions.TextBlockById, textBlock.Id, textBlock);
		}

		public void DeleteTextBlock(long id)
		{
			TextBlockDao.Delete(id);
			CacheManager.Remove(CacheRegions.TextBlockById, id);
		}

		public void DeleteTextBlock(string name)
		{
			TextBlock tb = GetTextBlock(name);
			if (tb != null)
			{
				TextBlockDao.Delete(tb.Id);
				CacheManager.Remove(CacheRegions.TextBlockById, tb.Id);
			}
		}

		public TextBlock GetTextBlock(long id)
		{
			TextBlock textBlock = (TextBlock)CacheManager.Get(CacheRegions.TextBlockById, id);
			if (textBlock == null)
			{
				if (KeepCacheLoaded)
				{
					InitializeTextBlocks();
					textBlock = (TextBlock)CacheManager.Get(CacheRegions.TextBlockById, id);
				}
				else
				{
					textBlock = TextBlockDao.Retrieve(id);
					CacheManager.Update(CacheRegions.TextBlockById, id, textBlock);
				}
				if (textBlock != null && textBlock.ConvertTextBlockXML())
				{
					UpdateTextBlock(textBlock);
				}
			}
			return textBlock;
		}

		public TextBlock GetTextBlock(string name)
		{
			TextBlock tb = TextBlockDao.Retrieve(name);
			if (tb != null && tb.ConvertTextBlockXML())
			{
				UpdateTextBlock(tb);
			}
			return tb;
		}

		public List<TextBlock> GetTextBlocks()
		{
			List<TextBlock> textBlocks = TextBlockDao.RetrieveAll();
			if (textBlocks != null && textBlocks.Count > 0)
			{
				foreach (TextBlock textBlock in textBlocks)
				{
					if (textBlock.ConvertTextBlockXML())
					{
						UpdateTextBlock(textBlock);
					}
				}
			}
			return textBlocks;
		}

		public List<TextBlock> GetTextBlocks(DateTime changedSince)
		{
			var result = TextBlockDao.RetrieveAll(changedSince) ?? new List<TextBlock>();
			if (result.Count > 0)
			{
				foreach (TextBlock textBlock in result)
				{
					if (textBlock.ConvertTextBlockXML())
					{
						UpdateTextBlock(textBlock);
					}
				}
			}
			return result;
		}

		#endregion

		#region Batches

		/// <summary>
		/// Allocate a new batch ID.
		/// </summary>
		/// <returns>batch ID</returns>
		public long GetNextBatchID()
		{
			using (var svc = LWDataServiceUtil.DataServiceInstance(Organization, Environment))
			{
				return svc.GetNextID(BATCHID_GENERATOR_NAME);
			}
		}

		/// <summary>
		/// Create a batch.  When the batchID is -1, a new batch ID will be allocated.  If the name
		/// is not provided, it will match the batch ID.
		/// </summary>
		/// <param name="batch">batch to create</param>
		/// <returns>new batch</returns>
		public void CreateBatch(Batch batch)
		{
			if (batch == null)
			{
				throw new ArgumentNullException("batch");
			}

			if (batch.ID == -1)
			{
				batch.ID = GetNextBatchID();
			}

			if (string.IsNullOrEmpty(batch.Name))
			{
				batch.Name = batch.ID.ToString();
			}

			if (BatchDao.Exists(batch.Name))
			{
				throw new LWException("A batch named '" + batch.Name + "' already exists");
			}

			BatchDao.Create(batch);
		}

		/// <summary>
		/// Update a batch.  If the name is not provided, it will match the batch ID. 
		/// </summary>
		/// <param name="batch">batch to update</param>
		/// <returns>updated batch</returns>
		public void UpdateBatch(Batch batch)
		{
			if (batch == null)
			{
				throw new ArgumentNullException("batch");
			}

			if (batch.ID < 0)
			{
				throw new ArgumentException("batch ID " + batch.ID + " is invalid");
			}

			if (string.IsNullOrEmpty(batch.Name))
			{
				batch.Name = batch.ID.ToString();
			}

			if (BatchDao.Exists(batch.Name, batch.ID))
			{
				throw new LWException("A batch named '" + batch.Name + "' already exists");
			}

			BatchDao.Update(batch);
		}

		/// <summary>
		/// Get a batch.
		/// </summary>
		/// <param name="batchID">batch ID</param>
		/// <returns>batch, or null if not found</returns>
		public Batch GetBatch(long batchID)
		{
			if (batchID < 0)
			{
				throw new ArgumentException("batch ID " + batchID + " is invalid");
			}
			return BatchDao.Retrieve(batchID);
		}

		/// <summary>
		/// Get a batch.
		/// </summary>
		/// <param name="batchName">batch name</param>
		/// <returns>batch, or null if not found</returns>
		public Batch GetBatch(string batchName)
		{
			if (string.IsNullOrEmpty(batchName))
			{
				throw new ArgumentNullException("batchName");
			}

			Batch result = (Batch)CacheManager.Get(CacheRegions.BatchByName, batchName);
			if (result == null)
			{
				result = BatchDao.Retrieve(batchName);
				if (result != null)
				{
					CacheManager.Update(CacheRegions.BatchByName, batchName, result);
				}
			}
			return result;
		}

		/// <summary>
		/// Get all batches.
		/// </summary>
		/// <returns>list of batches</returns>
		public List<Batch> GetBatches()
		{
			return BatchDao.RetrieveAll() ?? new List<Batch>();
		}

		/// <summary>
		/// Get all batches.
		/// </summary>
		/// <param name="changedSince">changed since date</param>
		/// <returns>list of batches</returns>
		public List<Batch> GetBatches(DateTime changedSince)
		{
			return BatchDao.RetrieveAll(changedSince) ?? new List<Batch>();
		}

		/// <summary>
		/// Get all active batches.
		/// </summary>
		/// <returns>list of batches</returns>
		public List<Batch> GetActiveBatches()
		{
			return BatchDao.RetrieveAllActive() ?? new List<Batch>();
		}

		/// <summary>
		/// Delete a batch.
		/// </summary>
		/// <param name="batchID">batch ID of the batch to delete</param>
		public void DeleteBatch(long batchID)
		{
			if (batchID < 0)
			{
				throw new ArgumentException("batch ID " + batchID + " is invalid");
			}

			using (var txn = Database.GetTransaction())
			{
				// Delete associated StructuredContentData's
				StructuredContentDataDao.DeleteBatch(batchID);

				// Delete Batch
				BatchDao.Delete(batchID);

				txn.Complete();
			}
		}

		#endregion

		#region Structured Content Elements

		public void CreateContentElement(StructuredContentElement element)
		{
			StructuredContentElementDao.Create(element);
		}

		public void UpdateContentElement(StructuredContentElement element)
		{
			StructuredContentElementDao.Update(element);
		}

		public void DeleteContentElement(long id)
		{
			using (var txn = Database.GetTransaction())
			{
				List<StructuredContentAttribute> attrs = StructuredContentAttributeDao.RetrieveByElementId(id);
				foreach (StructuredContentAttribute attr in attrs)
				{
					StructuredContentAttributeDao.Delete(attr.ID);
				}
				StructuredContentElementDao.Delete(id);
				txn.Complete();
			}
		}

		public bool IsDeletableContentElement(long elementID)
		{
			bool result = true;

			// Is the element referenced by content areas in a template?
			List<Template> templates = GetTemplates();
			foreach (Template template in templates)
			{
				List<TemplateContentArea> areas = template.GetHtmlContentAreas();
				areas.AddRange(template.GetHtmlDynamicAreas());
				areas.AddRange(template.GetTextContentAreas());
				foreach (TemplateContentArea area in areas)
				{
					long areaElementID = StringUtils.FriendlyInt64(area.StructuredElementId, -1);
					if (areaElementID == elementID)
					{
						result = false;
						break;
					}
				}
				if (!result)
				{
					break;
				}
			}

			// Is there a batch that contains data for the element?
			if (result && StructuredContentDataDao.HasDataForElementID(elementID))
			{
				result = false;
			}

			return result;
		}

		public StructuredContentElement GetContentElement(long id)
		{
			StructuredContentElement element = (StructuredContentElement)CacheManager.Get(CacheRegions.ContentElementById, id);
			if (element == null)
			{
				if (KeepCacheLoaded)
				{
					InitializeStructuredContentMeta();
					element = (StructuredContentElement)CacheManager.Get(CacheRegions.ContentElementById, id);
				}
				else
				{
					element = StructuredContentElementDao.Retrieve(id);
				}
			}
			return element;
		}

		public StructuredContentElement GetContentElement(string name)
		{
			StructuredContentElement element = (StructuredContentElement)CacheManager.Get(CacheRegions.ContentElementByName, name);
			if (element == null)
			{
				if (KeepCacheLoaded)
				{
					InitializeStructuredContentMeta();
					element = (StructuredContentElement)CacheManager.Get(CacheRegions.ContentElementByName, name);
				}
				else
				{
					element = StructuredContentElementDao.Retrieve(name);
				}
			}
			return element;
		}

		public List<StructuredContentElement> GetAllContentElements()
		{
			return StructuredContentElementDao.RetrieveAll() ?? new List<StructuredContentElement>();
		}

		public List<StructuredContentElement> GetAllContentElements(DateTime changedSince)
		{
			return StructuredContentElementDao.RetrieveAll(changedSince) ?? new List<StructuredContentElement>();
		}

		public List<StructuredContentElement> GetAllContentElementsInTemplate(long templateID)
		{
			var result = new List<StructuredContentElement>();
			Template template = GetTemplate(templateID);
			List<TemplateContentArea> areas = template.GetHtmlContentAreas();
			areas.AddRange(template.GetHtmlDynamicAreas());
			areas.AddRange(template.GetTextContentAreas());
			foreach (TemplateContentArea area in areas)
			{
				long elementID = StringUtils.FriendlyInt64(area.StructuredElementId, -1);
				if (elementID != -1)
				{
					bool found = false;
					foreach (StructuredContentElement element in result)
					{
						if (element.ID == elementID)
						{
							found = true;
							break;
						}
					}
					if (!found)
					{
						StructuredContentElement element = StructuredContentElementDao.Retrieve(elementID);
						result.Add(element);
					}
				}
			}
			return result;
		}

		#endregion

		#region Structured Content Attributes

		public void CreateAttribute(StructuredContentAttribute attribute)
		{
			StructuredContentAttributeDao.Create(attribute);
		}

		public void UpdateAttribute(StructuredContentAttribute attribute)
		{
			StructuredContentAttributeDao.Update(attribute);
		}

		public void DeleteAttribute(long id)
		{
			StructuredContentAttributeDao.Delete(id);
		}

		public StructuredContentAttribute GetAttribute(long id)
		{
			StructuredContentAttribute att = (StructuredContentAttribute)CacheManager.Get(CacheRegions.ContentAttributeById, id);
			if (att == null)
			{
				if (KeepCacheLoaded)
				{
					InitializeStructuredContentMeta();
					att = (StructuredContentAttribute)CacheManager.Get(CacheRegions.ContentAttributeById, id);
				}
				else
				{
					att = StructuredContentAttributeDao.Retrieve(id);
				}
			}
			return att;
		}

		public StructuredContentAttribute GetAttribute(string elementName, string attributeName)
		{
			// Is it a global attribute?
			StructuredContentAttribute attribute = GetGlobalAttribute(attributeName);
			if (attribute == null)
			{
				if (!string.IsNullOrWhiteSpace(elementName))
				{
					// Is it an element attribute?
					StructuredContentElement element = GetContentElement(elementName);
					if (element != null)
					{
						attribute = GetElementAttribute(attributeName, element.ID);
					}
				}
				else
				{
					// elementName not provided, so is there an element attribute with the given attributeName?
					var attrs = GetAllAttributes();
					if (attrs != null)
					{
						foreach (var attr in attrs)
						{
							if (attr.Name == attributeName)
							{
								attribute = attr;
								break;
							}
						}
					}
				}
			}
			return attribute;
		}

		public StructuredContentAttribute GetGlobalAttribute(long id)
		{
			return StructuredContentAttributeDao.RetrieveGlobal(id);
		}

		public StructuredContentAttribute GetGlobalAttribute(string name)
		{
			StructuredContentAttribute att = (StructuredContentAttribute)CacheManager.Get(CacheRegions.GlobalAttributeByName, name);
			if (att == null)
			{
				if (KeepCacheLoaded)
				{
					InitializeStructuredContentMeta();
					att = (StructuredContentAttribute)CacheManager.Get(CacheRegions.GlobalAttributeByName, name);
				}
				else
				{
					att = StructuredContentAttributeDao.RetrieveGlobal(name);
				}
			}
			return att;
		}

		public StructuredContentAttribute GetElementAttribute(string name, long elementID)
		{
			StructuredContentAttribute result = null;
			StructuredContentElement element = (StructuredContentElement)CacheManager.Get(CacheRegions.ContentElementById, elementID);
			if (element == null)
			{
				if (KeepCacheLoaded)
				{
					InitializeStructuredContentMeta();
					element = (StructuredContentElement)CacheManager.Get(CacheRegions.ContentElementById, elementID);
				}
				else
				{
					result = StructuredContentAttributeDao.RetrieveByNameAndElementId(name, elementID);
					return result;
				}
			}
			if (element != null)
			{
				foreach (StructuredContentAttribute att in element.Attributes)
				{
					if (att.Name == name)
					{
						result = att;
						break;
					}
				}
			}
			return result;
		}

		public List<StructuredContentAttribute> GetAllAttributes()
		{
			return StructuredContentAttributeDao.RetrieveAll() ?? new List<StructuredContentAttribute>();
		}

		public List<StructuredContentAttribute> GetAllAttributes(DateTime changedSince)
		{
			return StructuredContentAttributeDao.RetrieveAll(changedSince) ?? new List<StructuredContentAttribute>();
		}

		public List<StructuredContentAttribute> GetGlobalAttributes()
		{
			List<StructuredContentAttribute> result = (List<StructuredContentAttribute>)CacheManager.Get(CacheRegions.GlobalAttributes, "list");
			if (result == null)
			{
				if (KeepCacheLoaded)
				{
					InitializeStructuredContentMeta();
					result = (List<StructuredContentAttribute>)CacheManager.Get(CacheRegions.GlobalAttributes, "list");
				}
				else
				{
					result = (List<StructuredContentAttribute>)StructuredContentAttributeDao.RetrieveAllGlobals();
					if (result == null)
					{
						result = new List<StructuredContentAttribute>();
					}
				}
			}
			return result;
		}

		public List<StructuredContentAttribute> GetElementAttributes(long elementID)
		{
			List<StructuredContentAttribute> result = null;
			StructuredContentElement element = (StructuredContentElement)CacheManager.Get(CacheRegions.ContentElementById, elementID);
			if (element == null)
			{
				if (KeepCacheLoaded)
				{
					InitializeStructuredContentMeta();
					element = (StructuredContentElement)CacheManager.Get(CacheRegions.ContentElementById, elementID);
					result = (List<StructuredContentAttribute>)element.Attributes;
				}
				else
				{
					result = (List<StructuredContentAttribute>)StructuredContentAttributeDao.RetrieveByElementId(elementID);
					if (result == null)
					{
						result = new List<StructuredContentAttribute>();
					}
				}
			}
			else
			{
				result = (List<StructuredContentAttribute>)element.Attributes;
			}
			return result;
		}

		#endregion

		#region Structured Content Data

		public long GetNextSequenceID(long batchID, long elementID)
		{
			return StructuredContentDataDao.GetNextSequenceID(batchID, elementID);
		}

		public void CreateDatum(StructuredContentData datum)
		{
			StructuredContentDataDao.Create(datum);
		}

		public void UpdateDatum(StructuredContentData datum)
		{
			StructuredContentDataDao.Update(datum);
		}

		public void DeleteDatum(long id)
		{
			StructuredContentDataDao.DeleteDatum(id);
		}

		public void DeleteDatumBatch(long id)
		{
			StructuredContentDataDao.DeleteBatch(id);
		}

		public void DeleteBatchSequence(long batchID, long sequenceID)
		{
			StructuredContentDataDao.DeleteBatchSequence(batchID, sequenceID);
		}

		public StructuredContentData GetDatum(long datumID)
		{
			return StructuredContentDataDao.Retrieve(datumID);
		}

		public StructuredContentData GetDatum(long batchID, long sequenceID, long attributeID)
		{
			return StructuredContentDataDao.Retrieve(batchID, sequenceID, attributeID);
		}

		public List<StructuredContentData> GetAllDatums()
		{
			return StructuredContentDataDao.RetrieveAll() ?? new List<StructuredContentData>();
		}

		public List<StructuredContentData> GetAllDatums(DateTime changedSince)
		{
			return StructuredContentDataDao.RetrieveAll(changedSince) ?? new List<StructuredContentData>();
		}

		public List<StructuredContentData> GetDataColumn(string attributeName)
		{
			return StructuredContentDataDao.RetrieveDataColumn(attributeName) ?? new List<StructuredContentData>();
		}

		/// <summary>
		/// Get rows of data.
		/// </summary>
		/// <param name="batchID">ID of the batch, or -1 for all batches</param>
		/// <param name="elementID">ID of the element</param>
		/// <param name="matchAttribute">attribute for matching the value</param>
		/// <param name="matchValue">attribute value that must match</param>
		/// <param name="unexpiredOnly">use only unexpired batches</param>
		/// <returns>table containing rows of data</returns>
		public DataTable GetDataRows(long batchID, long elementID, StructuredContentAttribute matchAttribute, string matchValue, bool unexpiredOnly)
		{
			DataTable result = CreateRowDataTable(elementID);

			// Get a list of datums that represent a subset of values in the specified attribute column
			// where the value matches the matchValue.
			List<StructuredContentData> matchingDatums = StructuredContentDataDao.RetrieveMatchingDatums(batchID, matchAttribute.ID, matchValue);
			if (matchingDatums.Count > 0)
			{
				// Preprocess
				//StructuredContentAttribute batchNameAttr = GetGlobalAttribute(StructuredContentAttribute.BATCH_NAME);
				StructuredContentAttribute batchStartDateAttr = GetGlobalAttribute(StructuredContentAttribute.START_DATE);
				StructuredContentAttribute batchEndDateAttr = GetGlobalAttribute(StructuredContentAttribute.END_DATE);
				Dictionary<long, string> globalID2Name = new Dictionary<long, string>();
				foreach (StructuredContentAttribute globalAttr in GetGlobalAttributes())
				{
					globalID2Name.Add(globalAttr.ID, globalAttr.Name);
				}
				Dictionary<long, string> elementID2Name = new Dictionary<long, string>();
				foreach (StructuredContentAttribute elementAttr in GetElementAttributes(elementID))
				{
					elementID2Name.Add(elementAttr.ID, elementAttr.Name);
				}

				// Process
				long lastBatchID = -1;
				List<StructuredContentData> globalDatums = null;
				foreach (StructuredContentData matchingDatum in matchingDatums)
				{
					// New batch
					if (matchingDatum.BatchID != lastBatchID)
					{
						// Check expiration
						if (unexpiredOnly)
						{
							StructuredContentData batchStartDateDatum = GetDatum(matchingDatum.BatchID, -1, batchStartDateAttr.ID);
							StructuredContentData batchEndDateDatum = GetDatum(matchingDatum.BatchID, -1, batchEndDateAttr.ID);
							DateTime batchStartDate = DateTimeUtil.ConvertStringToDate(null, batchStartDateDatum.Data);
							DateTime batchEndDate = DateTimeUtil.ConvertStringToDate(null, batchEndDateDatum.Data);
							DateTime now = DateTime.Now;
							if (batchStartDate.CompareTo(now) > 0 || batchEndDate.CompareTo(now) <= 0)
							{
								// expired, so go to the next batch
								globalDatums = null;
								continue;
							}
						}

						// Get globals
						globalDatums = StructuredContentDataDao.RetrieveGlobalData(matchingDatum.BatchID);
					}

					if (globalDatums != null)
					{
						List<StructuredContentData> elementDatums = StructuredContentDataDao.RetrieveElementData(matchingDatum.BatchID, elementID);
						if (elementDatums != null && elementDatums.Count > 0)
						{
							DataRow row = result.NewRow();
							long lastSeqID = -2;
							foreach (StructuredContentData elementDatum in elementDatums)
							{
								if (elementDatum.SequenceID != lastSeqID)
								{
									if (lastSeqID != -2)
									{
										if (row[matchAttribute.Name] != null && row[matchAttribute.Name].ToString() == matchValue)
										{
											result.Rows.Add(row);
										}
										row = result.NewRow();
									}

									// Add global values
									foreach (StructuredContentData globalDatum in globalDatums)
									{
										string globColumnName = globalID2Name[globalDatum.AttributeID];
										row[globColumnName] = globalDatum.Data;
									}
								}

								// Add element values
								string elementColumnName = elementID2Name[elementDatum.AttributeID];
								row[elementColumnName] = elementDatum.Data;

								lastSeqID = elementDatum.SequenceID;
							}
							if (row[matchAttribute.Name] != null && row[matchAttribute.Name].ToString() == matchValue)
							{
								result.Rows.Add(row);
							}
						}
					}

					lastBatchID = matchingDatum.BatchID;
				}
			}

			return result;
		}

		public List<StructuredContentData> GetDatumBatch(long id)
		{
			return StructuredContentDataDao.RetrieveBatch(id) ?? new List<StructuredContentData>();
		}

		public List<StructuredContentData> GetDatumBatch(string name)
		{
			return StructuredContentDataDao.RetrieveBatch(name) ?? new List<StructuredContentData>();
		}

		public SortedList<string, long> GetDatumBatches()
		{
			return StructuredContentDataDao.RetrieveBatches();
		}

		public List<StructuredContentData> GetBatchGlobals(long batchID)
		{
			List<StructuredContentData> list = (List<StructuredContentData>)CacheManager.Get(CacheRegions.BatchGlobals, batchID);
			if (list == null)
			{
				list = (List<StructuredContentData>)StructuredContentDataDao.RetrieveGlobalData(batchID) ?? new List<StructuredContentData>();
				CacheManager.Add(CacheRegions.BatchGlobals, batchID, list);
			}
			return list;
		}

		public List<StructuredContentData> GetBatchElements(long batchID, long elementID)
		{
			string key = MakeKey(batchID, elementID);
			List<StructuredContentData> list = (List<StructuredContentData>)CacheManager.Get(CacheRegions.BatchElements, key);
			if (list == null)
			{
				StructuredContentElement element = GetContentElement(elementID);

				if (KeepCacheLoaded == false && element != null && element.Attributes == null)
				{
					element.Attributes = StructuredContentAttributeDao.RetrieveByElementId(elementID);
				}

				// get list of attribute ids
				if (element != null && element.Attributes != null && element.Attributes.Count > 0)
				{
					var ids = from x in element.Attributes select x.ID;
					list = (List<StructuredContentData>)StructuredContentDataDao.RetrieveElementData(batchID, ids.ToArray<long>());
					if (list == null)
					{
						list = new List<StructuredContentData>();
					}
					CacheManager.Add(CacheRegions.BatchElements, key, list);
				}
			}
			return list;
		}

		public SearchDatumResult SearchDatums(SearchDatumArgs args)
		{
			List<StructuredContentData> matchingDatums = (List<StructuredContentData>)StructuredContentDataDao.Search(args);

			SearchDatumResult result = new SearchDatumResult();
			foreach (StructuredContentData matchingDatum in matchingDatums)
			{
				SearchDatumResultBatch sdrBatch = result.FindBatch(matchingDatum.BatchID);
				if (sdrBatch == null)
				{
					sdrBatch = result.CreateBatch(BatchDao.Retrieve(matchingDatum.BatchID));
				}
				SearchDatumResultItem match = new SearchDatumResultItem();
				match.SearchExpression = args.SearchExpression;
				match.CaseSensitiveSearch = args.IsCaseSensitiveSearch;
				match.MatchingDatum = matchingDatum;
				//match.FilterRow = _dataDao.RetrieveFilteredRow(matchingDatum.BatchID, matchingDatum.SequenceID, elementID);
				match.DataRow = StructuredContentDataDao.RetrieveRow(matchingDatum.BatchID, matchingDatum.SequenceID, args.ElementID);
				sdrBatch.AddMatch(match);
			}
			return result;
		}

		public FilterCollection GetFiltersForElement(long elementID)
		{
			FilterCollection result = new FilterCollection();
			List<StructuredContentAttribute> attrs = StructuredContentAttributeDao.RetrieveAllFiltersForElement(elementID);
			if (attrs != null)
			{
				foreach (StructuredContentAttribute attr in attrs)
				{
					Filter filter = new Filter();
					filter.AttributeID = attr.ID;
					filter.AttributeName = attr.Name;
					//todo: filter order doesn't need to be a long - 2,147,483,647 filters for a 
					//single content area is probably enough for anybody. casting as int for now, 
					filter.FilterOrder = (int)attr.FilterOrder;
					filter.ListField = attr.ListField;
					filter.Add(string.Empty);
					result.Add(filter);
				}
			}
			return result;
		}

		public FilterCollection GetFilterCollection(long batchID, long elementID)
		{
			FilterCollection result = StructuredContentDataDao.RetrieveFilterCollection(batchID, elementID);
			if (result == null || result.Count < 1)
			{
				result = GetFiltersForElement(elementID);
			}
			return result;
		}

		public StructuredDataRowList GetFilteredAttributes(long batchID, long elementID, Dictionary<long, string> filterValues)
		{
			StructuredDataRowList result = StructuredContentDataDao.RetrieveFilteredAttributes(batchID, elementID, filterValues);
			return result;
		}

		#endregion

		#region Category Management

		public void AddCategory(Category category)
		{
			_logger.Trace(_className, "AddCategory", "Creating new category " + category.Name);
			CategoryDao.Create(category);
		}

		public void UpdateCategory(Category category)
		{
			CategoryDao.Update(category);
		}

		public List<Category> GetCategories()
		{
			return CategoryDao.RetrieveAll() ?? new List<Category>();
		}

		public List<Category> GetCategories(bool isVisibleInLN)
		{
            List<Category> categories = CacheManager.Get(CacheRegions.Categories, "all") as List<Category>;
		    if (categories == null)
		    {
                categories = CategoryDao.RetrieveByVisibility(isVisibleInLN) ?? new List<Category>();
		        CacheManager.Update(CacheRegions.Categories, "all", categories);
                foreach (Category category in categories)
                {
                    CacheManager.Update(CacheRegions.CategoryById, category.ID, category);
                }
            }
		    return categories;
		}

		public List<Category> GetCategories(long[] ids)
		{
            List<Category> categories = CategoryDao.RetrieveCategoriesByIds(ids) ?? new List<Category>();
            foreach (Category category in categories)
            {
                CacheManager.Update(CacheRegions.CategoryById, category.ID, category);
            }
            return categories;
        }

		public List<Category> GetTopLevelCategories(bool isVisibleInLN)
		{
			return CategoryDao.RetrieveChildCategoriesAll(0L, isVisibleInLN) ?? new List<Category>();
		}

		public List<Category> GetTopLevelCategoriesByType(CategoryType type, bool isVisibleInLN)
		{
			return CategoryDao.RetrieveTopLevelCategoriesByType(type, isVisibleInLN) ?? new List<Category>();
		}

		public List<Category> GetChildCategories(long categoryId, bool isVisibleInLN)
		{
			return GetChildCategories(categoryId, isVisibleInLN, null);
		}

		public List<Category> GetChildCategories(long categoryId, bool isVisibleInLN, LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null) batchInfo.Validate();
			return CategoryDao.RetrieveChildCategoriesAll(categoryId, isVisibleInLN, batchInfo) ?? new List<Category>();
		}

		public List<Category> GetAllChangedCategories(DateTime since)
		{
			return CategoryDao.RetrieveChangedObjects(since) ?? new List<Category>();
		}

		public Category GetCategory(long categoryId)
		{
            Category category = (Category)CacheManager.Get(CacheRegions.CategoryById, categoryId);
            if (category == null)
            {
                category = CategoryDao.Retrieve(categoryId);
                if (category != null)
                {
                    CacheManager.Update(CacheRegions.CategoryById, category.ID, category);
                }
            }
            return category;
		}

		public Category GetCategory(long parentId, string catName)
		{
			return CategoryDao.Retrieve(parentId, catName);
		}

		public List<Category> GetCategoriesByType(CategoryType type, bool isVisibleInLN)
		{
			return CategoryDao.RetrieveByType(type, isVisibleInLN) ?? new List<Category>();
		}

		public void DeleteCategory(long categoryId)
		{
			_logger.Trace(_className, "DeleteCategory", "Deleting category with id = " + categoryId);
			CategoryDao.Delete(categoryId);
		}

		#endregion

		#region Langauges

		public LanguageDef GetLanguageDef(string culture)
		{
			LanguageDef lang = null;
			List<LanguageDef> langs = GetLanguageDefs();
			var lan = (from x in langs where x.Culture == culture select x);
			if (lan != null && lan.Count() > 0)
			{
				lang = lan.ElementAt(0) as LanguageDef;
			}
			return lang;
		}

		public List<LanguageDef> GetLanguageDefs()
		{
			var langs = (List<LanguageDef>)CacheManager.Get(CacheRegions.Languages, "All");
			if (langs == null)
			{
				langs = LanguageDao.RetrieveAll() ?? new List<LanguageDef>();
				CacheManager.Update(CacheRegions.Languages, "All", langs);
			}
			return langs;
		}

		public bool IsLanguageSupported(string culture)
		{
			LanguageDef lang = GetLanguageDef(culture);
			if (lang == null)
			{
				string groupCulture = LanguageChannelUtil.GetGroupCulture(culture);
				if (!string.IsNullOrEmpty(groupCulture))
				{
					lang = GetLanguageDef(groupCulture);
				}
			}
			return lang != null ? true : false;
		}

		#endregion

		#region Channels

		public ChannelDef GetChannelDef(string name)
		{
			ChannelDef channel = null;
			List<ChannelDef> channels = GetChannelDefs();
			var chan = (from x in channels where x.Name == name select x);
			if (chan != null && chan.Count() > 0)
			{
				channel = chan.ElementAt(0) as ChannelDef;
			}
			return channel;
		}


		public virtual ChannelDef GetChannelDef(long id)
		{
			ChannelDef channel = null;
			List<ChannelDef> channels = GetChannelDefs();
			var chan = (from x in channels where x.Id == id select x);
			if (chan != null && chan.Count() > 0)
			{
				channel = chan.ElementAt(0) as ChannelDef;
			}
			return channel;
		}

		public List<ChannelDef> GetChannelDefs()
		{
			List<ChannelDef> channels = (List<ChannelDef>)CacheManager.Get(CacheRegions.Channels, "All");
			if (channels == null)
			{
				channels = ChannelDao.RetrieveAll() ?? new List<ChannelDef>();
				CacheManager.Update(CacheRegions.Channels, "All", channels);
			}
			return channels;
		}

		public bool IsChannelSupported(string name)
		{
			ChannelDef channel = GetChannelDef(name);
			return channel != null ? true : false;
		}

		public void CreateChannelDef(ChannelDef channel)
		{
			ChannelDao.Create(channel);
			CacheManager.Remove(CacheRegions.Channels, "All");
		}

		public void UpdateChannelDef(ChannelDef channel)
		{
			ChannelDao.Update(channel);
			CacheManager.Remove(CacheRegions.Channels, "All");
		}

		public void DeleteChannelDef(string name)
		{
			ChannelDef channel = GetChannelDef(name);
			if (channel != null)
			{
				ChannelDao.Delete(name);
				CacheManager.Remove(CacheRegions.Channels, "All");
			}
		}

		public void DeleteChannelDef(long id)
		{
			ChannelDao.Delete(id);
			CacheManager.Remove(CacheRegions.Channels, "All");
		}

		#endregion

		public virtual List<LangChanContent> GetLangChanContents(ContentObjType? type, LanguageDef language, ChannelDef channel)
		{
			return LangChanContentDao.Retrieve(type, language, channel);
		}


		#region Store Def

		public void CreateStoreDef(StoreDef store)
		{
			StoreDao.Create(store);
		}

		public void CreateStoreDef(List<StoreDef> stores)
		{
			StoreDao.Create(stores);
		}

		public void UpdateStoreDef(StoreDef store)
		{
			StoreDao.Update(store);
		}

		public void UpdateStoreDef(List<StoreDef> stores)
		{
			StoreDao.Update(stores);
		}

		public StoreDef GetStoreDef(long storId)
		{
			return StoreDao.Retrieve(storId);
		}

		public List<StoreDef> GetStoreDef(string storeNmbr)
		{
			return StoreDao.Retrieve(storeNmbr) ?? new List<StoreDef>();
		}

		public StoreDef GetStoreDefByStoreNumberAndBrand(string storeNmbr, string brandName)
		{
			return StoreDao.RetrieveByStoreNumberAndBrand(storeNmbr, brandName);
		}

		public StoreDef GetStoreDefByBrand(string brandName, string brandStoreNmbr)
		{
			return StoreDao.RetrieveByBrandStore(brandName, brandStoreNmbr);
		}

		public List<StoreDef> GetAllStoreDefs(LWQueryBatchInfo batchInfo = null)
		{
			return StoreDao.RetrieveAll(batchInfo) ?? new List<StoreDef>();
		}

		public List<StoreDef> GetAllStoreDefs(long[] storeIds)
		{
			return StoreDao.RetrieveAll(storeIds) ?? new List<StoreDef>();
		}

		public NearbyStoreCollection GetStoreDefsNearby(double latitude, double longitude, double radiusInMiles, int maxRows)
		{
			return StoreDao.RetrieveNearby(latitude, longitude, radiusInMiles, maxRows) ?? new NearbyStoreCollection();
		}

		public List<StoreDef> GetStoreDefsByCityAndStateOrProvince(string city, string stateOrProvince, int maxRows)
		{
			return StoreDao.RetrieveAllByCityAndStateOrProvince(city, stateOrProvince, maxRows) ?? new List<StoreDef>();
		}

		public List<StoreDef> GetStoreDefsByZipOrPostalCode(string zipOrPostalCode, int maxRows)
		{
			return StoreDao.RetrieveAllByZipOrPostalCode(zipOrPostalCode, maxRows) ?? new List<StoreDef>();
		}

		public List<StoreDef> GetStoreDefsByUserField(long userField)
		{
			return StoreDao.RetrieveByUserField(userField) ?? new List<StoreDef>();
		}

		public List<StoreDef> GetStoreDefsByUserField(string userField)
		{
			return StoreDao.RetrieveByUserField(userField) ?? new List<StoreDef>();
		}

		public List<StoreDef> GetAllChangedStoreDefs(DateTime since)
		{
			return StoreDao.RetrieveChangedObjects(since) ?? new List<StoreDef>();
		}

		public Dictionary<long, string> GetStoreDefsByProperty(string propertyName, string whereClause)
		{
			return StoreDao.RetrieveByProperty(propertyName, whereClause) ?? new Dictionary<long, string>();
		}

		public void DeleteStoreDef(long storeNmbr)
		{
			StoreDao.Delete(storeNmbr);
		}

		#endregion

		#region Promotion

		public void CreatePromotion(Promotion promotion)
		{
			_logger.Trace(_className, "CreatePromotion", "Creating new promotion " + promotion.Name);
			PromotionDao.Create(promotion);
			CacheManager.Update(CacheRegions.PromotionByCode, promotion.Code, promotion);
		}

		public void UpdatePromotion(Promotion promotion)
		{
			PromotionDao.Update(promotion);
			CacheManager.Update(CacheRegions.PromotionByCode, promotion.Code, promotion);
		}

		public Promotion GetPromotion(long promotionId)
		{
			return PromotionDao.Retrieve(promotionId);
		}

		public Promotion GetPromotionByCode(string code)
		{
			var promo = (Promotion)CacheManager.Get(CacheRegions.PromotionByCode, code);
			if (promo == null)
			{
				promo = PromotionDao.RetrieveByCode(code);
				if (promo != null)
				{
					CacheManager.Update(CacheRegions.PromotionByCode, promo.Code, promo);
				}
			}
			return promo;
		}

		public Promotion GetPromotionByName(string name)
		{
			return PromotionDao.RetrieveByName(name);
		}

		public int HowManyPromotions(List<Dictionary<string, object>> parms)
		{
			return PromotionDao.HowManyPromotions(parms);
		}

		public List<Promotion> GetAllPromotions(LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null) batchInfo.Validate();
			return PromotionDao.RetrieveAll(batchInfo) ?? new List<Promotion>();
		}

		public List<Promotion> GetUnexpiredPromotions()
		{
			List<Dictionary<string, object>> parms = new List<Dictionary<string, object>>();
			Dictionary<string, object> entry = new Dictionary<string, object>();
			entry.Add("Property", "Unexpired");
			entry.Add("Predicate", LWCriterion.Predicate.Eq);
			entry.Add("Value", true);
			parms.Add(entry);
			return PromotionDao.RetrievePromotions(parms, false, null) ?? new List<Promotion>();
		}

		public List<Promotion> GetPromotionIds(long[] ids)
		{
			return PromotionDao.Retrieve(ids) ?? new List<Promotion>();
		}

		public List<Promotion> GetAllChangedPromotions(DateTime since)
		{
			return PromotionDao.RetrieveChangedObjects(since) ?? new List<Promotion>();
		}

		public List<long> GetPromotionIds(bool activeOnly, string sortExpression, bool ascending, long? folderId = null)
		{
			List<Dictionary<string, object>> parmsList = new List<Dictionary<string, object>>();
			if (activeOnly)
			{
				Dictionary<string, object> parms = new Dictionary<string, object>();
				parms.Add("Property", "Active");
				parms.Add("Predicate", LWCriterion.Predicate.Eq);
				parms.Add("Value", true);
				parmsList.Add(parms);
			}
			return PromotionDao.RetrievePromotionIds(parmsList, sortExpression, ascending, folderId);
		}

		public List<long> GetPromotionIds(List<Dictionary<string, object>> parms, string sortExpression, bool ascending, long? folderId = null)
		{
			return PromotionDao.RetrievePromotionIds(parms, sortExpression, ascending, folderId);
		}

		public List<Promotion> GetPromotions(List<Dictionary<string, object>> parms, bool populateAttributes, LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null) batchInfo.Validate();
			return PromotionDao.RetrievePromotions(parms, populateAttributes, batchInfo) ?? new List<Promotion>();
		}

		public Dictionary<long, string> GetPromotionsByProperty(string propertyName, string whereClause)
		{
			return PromotionDao.RetrieveByProperty(propertyName, whereClause) ?? new Dictionary<long, string>();
		}

		public void DeletePromotion(long promotionId)
		{
			_logger.Trace(_className, "DeletePromotion", "Deleting promotion with id = " + promotionId);

			//for some reason, member promotions and rule triggers tie to the promotion by code instead of Id. Because of this, we have
			//to cache promotions by code for performance. In order to remove the promotion from cache, we have to look up the promotion 
			//first to get its code. Ideally, triggers and member promotions would use the id of the promotion, not the code.
			if (CacheManager.RegionExists(CacheRegions.PromotionByCode))
			{
				var promo = GetPromotion(promotionId);
				if (promo != null)
				{
					CacheManager.Remove(CacheRegions.PromotionByCode, promo.Id);
				}
			}

			PromotionDao.Delete(promotionId);
		}

		#endregion

		#region BonusDef

		public void CreateBonusDef(BonusDef bonus)
		{
			BonusDao.Create(bonus);
			CacheManager.Update(CacheRegions.BonusById, bonus.Id, bonus);
		}

		public void UpdateBonusDef(BonusDef bonus)
		{
			BonusDao.Update(bonus);
			CacheManager.Update(CacheRegions.BonusById, bonus.Id, bonus);
		}

		public BonusDef GetBonusDef(long id)
		{
			BonusDef bonus = (BonusDef)CacheManager.Get(CacheRegions.BonusById, id);
			if (bonus == null)
			{
				bonus = BonusDao.Retrieve(id);
				if (bonus != null)
				{
					CacheManager.Update(CacheRegions.BonusById, bonus.Id, bonus);
				}
			}
			return bonus;
		}

		public BonusDef GetBonusDef(string name)
		{
			BonusDef bonus = (BonusDef)CacheManager.Get(CacheRegions.BonusByName, name);
			if (bonus == null)
			{
				bonus = BonusDao.Retrieve(name);
				if (bonus != null)
				{
					CacheManager.Update(CacheRegions.BonusByName, bonus.Name, bonus);
				}
			}
			return bonus;
		}

		public List<BonusDef> GetBonusDefs(long[] ids, bool populateAttributes)
		{
			return BonusDao.Retrieve(ids, populateAttributes) ?? new List<BonusDef>();
		}

		public List<BonusDef> GetBonusDefsByCategory(long categoryId, bool includeChildren, bool excludeExpired = true)
		{
			return GetBonusDefsByCategory(null, categoryId, includeChildren, excludeExpired);
		}

		private List<BonusDef> GetBonusDefsByCategory(List<BonusDef> list, long categoryId, bool includeChildren, bool excludeExpired = true)
		{
			if (list == null)
			{
				list = new List<BonusDef>();
			}
			if (includeChildren)
			{
				List<Category> childCats = GetChildCategories(categoryId, true);
				foreach (Category cat in childCats)
				{
					list = GetBonusDefsByCategory(list, cat.ID, includeChildren, excludeExpired);
				}
			}
			List<BonusDef> bonuses = BonusDao.RetrieveByCategory(categoryId, excludeExpired);
			if (bonuses != null)
			{
				foreach (BonusDef bonus in bonuses)
				{
					list.Add(bonus);
					CacheManager.Update(CacheRegions.BonusById, bonus.Id, bonus);
				}
			}
			return list;
		}

		public List<BonusDef> GetAllChangedBonusDefs(DateTime since)
		{
			return BonusDao.RetrieveChangedObjects(since) ?? new List<BonusDef>();
		}

		public List<BonusDef> GetAllBonusDefs()
		{
			var ret = BonusDao.RetrieveAll() ?? new List<BonusDef>();
			foreach (BonusDef ad in ret)
			{
				CacheManager.Update(CacheRegions.BonusById, ad.Id, ad);
			}
			return ret;
		}

		public List<BonusDef> GetUnexpiredBonusDefs()
		{
			List<Dictionary<string, object>> parms = new List<Dictionary<string, object>>();
			Dictionary<string, object> entry = new Dictionary<string, object>();
			entry.Add("Property", "Unexpired");
			entry.Add("Predicate", LWCriterion.Predicate.Eq);
			entry.Add("Value", true);
			parms.Add(entry);

			List<BonusDef> bonuses = BonusDao.RetrieveBonusDefs(parms, false, null);
			if (bonuses == null)
			{
				bonuses = new List<BonusDef>();
			}
			return bonuses;
		}

		public long CheckAndUpdateBonusQuotaCount(long bonusId, long completed)
		{
			BonusDef def = GetBonusDef(bonusId);
			long newCount = BonusDao.CheckAndUpdateQuotaCount(bonusId, completed);
			if (newCount > 0)
			{
				CacheManager.Remove(CacheRegions.BonusById, def.Id);
			}
			return newCount;
		}

		public int HowManyBonusDefs(List<Dictionary<string, object>> parms)
		{
			return BonusDao.HowManyBonuses(parms);
		}

		public List<long> GetBonusDefIds(List<Dictionary<string, object>> parms, string sortExpression, bool ascending, long? folderId = null)
		{
			return BonusDao.RetrieveBonusDefIds(parms, sortExpression, ascending, folderId);
		}

		public List<BonusDef> GetBonusDefs(List<Dictionary<string, object>> parms, bool populateAttributes, LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null) batchInfo.Validate();
			return BonusDao.RetrieveBonusDefs(parms, populateAttributes, batchInfo) ?? new List<BonusDef>();
		}

		public void DeleteBonusDef(long id)
		{
			BonusDao.Delete(id);
			CacheManager.Remove(CacheRegions.BonusById, id);
		}

		#endregion

		#region CouponDef

		public void CreateCouponDef(CouponDef coupon)
		{
			CouponDao.Create(coupon);
			CacheManager.Update(CacheRegions.CouponByName, coupon.Name, coupon);
		}

		public void CreateCouponDefs(List<CouponDef> coupons)
		{
			CouponDao.Create(coupons);
		}

		public void UpdateCouponDef(CouponDef coupon)
		{
			CouponDao.Update(coupon);
			CacheManager.Update(CacheRegions.CouponByName, coupon.Name, coupon);
		}

		public void UpdateCouponDefs(List<CouponDef> coupons)
		{
			CouponDao.Update(coupons);
		}

		public CouponDef GetCouponDef(long couponId)
		{
			CouponDef coupon = CouponDao.Retrieve(couponId);
			if (coupon != null)
			{
				CacheManager.Update(CacheRegions.CouponByName, coupon.Name, coupon);
			}
			return coupon;
		}

		public CouponDef GetCouponDef(string name)
		{
			CouponDef coupon = (CouponDef)CacheManager.Get(CacheRegions.CouponByName, name);
			if (coupon == null)
			{
				coupon = CouponDao.Retrieve(name);
				if (coupon != null)
				{
					CacheManager.Update(CacheRegions.CouponByName, coupon.Name, coupon);
				}
			}
			return coupon;
		}

		public CouponDef GetCouponDefByCode(string code)
		{
			CouponDef coupon = CouponDao.RetrieveByCode(code);
			if (coupon != null)
			{
				CacheManager.Update(CacheRegions.CouponByName, coupon.Name, coupon);
			}
			return coupon;
		}

		public int HowManyCouponDefs(List<Dictionary<string, object>> parms, ActiveCouponOptions options = null)
		{
			return CouponDao.Count(parms, options);
		}

		public List<CouponDef> GetCouponDefs(long[] ids, bool populateAttributes)
		{
			return CouponDao.Retrieve(ids, populateAttributes) ?? new List<CouponDef>();
		}

		public List<long> GetCouponDefIds(List<Dictionary<string, object>> parms, string sortExpression, bool ascending, long? folderId = null)
		{
			return CouponDao.RetrieveCouponDefIds(parms, sortExpression, ascending, folderId) ?? new List<long>();
		}

		public List<CouponDef> GetCouponDefs(List<Dictionary<string, object>> parms, bool populateAttributes, LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null) batchInfo.Validate();
			return CouponDao.RetrieveCouponDefs(parms, populateAttributes, batchInfo) ?? new List<CouponDef>();
		}

		public PetaPoco.Page<CouponDef> GetCouponDefs(List<Dictionary<string, object>> parms, ActiveCouponOptions options, bool populateAttributes, int pageNumber, int resultsPerPage)
		{
			return CouponDao.Retrieve(parms, options, populateAttributes, null, pageNumber, resultsPerPage);
		}

		public List<CouponDef> GetAllCouponDefs()
		{
			var coupons = CouponDao.RetrieveAll() ?? new List<CouponDef>();
			foreach (CouponDef coupon in coupons)
			{
				CacheManager.Update(CacheRegions.CouponByName, coupon.Name, coupon);
			}
			return coupons;
		}

		public List<CouponDef> GetUnexpiredCouponDefs()
		{
			List<Dictionary<string, object>> parms = new List<Dictionary<string, object>>();
			Dictionary<string, object> entry = new Dictionary<string, object>();
			entry.Add("Property", "Unexpired");
			entry.Add("Predicate", LWCriterion.Predicate.Eq);
			entry.Add("Value", true);
			parms.Add(entry);

			return CouponDao.RetrieveCouponDefs(parms, false, null) ?? new List<CouponDef>();
		}

		private List<CouponDef> GetCouponDefsByCategory(List<CouponDef> list, long categoryId, bool includeChildren)
		{
			if (list == null)
			{
				list = new List<CouponDef>();
			}
			if (includeChildren)
			{
				List<Category> childCats = GetChildCategories(categoryId, true);
				foreach (Category cat in childCats)
				{
					list = GetCouponDefsByCategory(list, cat.ID, includeChildren);
				}
			}
			List<CouponDef> coupons = CouponDao.RetrieveByCategory(categoryId);
			if (coupons != null)
			{
				foreach (CouponDef coupon in coupons)
				{
					list.Add(coupon);
				}
			}
			return list;
		}

		public List<CouponDef> GetCouponDefsByCategory(long categoryId, bool includeChildren)
		{
			return GetCouponDefsByCategory(null, categoryId, includeChildren);
		}

		public List<CouponDef> GetAllChangedCouponDefs(DateTime since)
		{
			return CouponDao.RetrieveChangedObjects(since) ?? new List<CouponDef>();
		}

		public void DeleteCouponDef(long couponId)
		{
			CouponDef coupon = GetCouponDef(couponId);
			if (coupon != null)
			{
				CouponDao.Delete(couponId);
				CacheManager.Remove(CacheRegions.CouponByName, coupon.Name);
			}
		}

		#endregion


        //RTW   10/07/2016  LW-2759 - Added Caching support to message definitions
		#region Customer Message Definition

		public void CreateMessageDef(MessageDef message)
		{
			_logger.Trace(_className, "CreateMessageDef", "Creating new customer message " + message.Name);
			MessageDao.Create(message);
            CacheManager.Update(CacheRegions.MessageByName, message.Name, message);
            CacheManager.Update(CacheRegions.MessageById, message.Id, message);
        }

        public void UpdateMessageDef(MessageDef message)
		{
			MessageDao.Update(message);
            CacheManager.Update(CacheRegions.MessageByName, message.Name, message);
            CacheManager.Update(CacheRegions.MessageById, message.Id, message);
        }

        public MessageDef GetMessageDef(long id)
		{
            MessageDef message = (MessageDef)CacheManager.Get(CacheRegions.MessageById, id);
            if (message == null)
            {
                message = MessageDao.Retrieve(id);
                if (message != null)
                {
                    CacheManager.Update(CacheRegions.MessageByName, message.Name, message);
                    CacheManager.Update(CacheRegions.MessageById, message.Id, message);
                }
            }

            return message;
		}

		public MessageDef GetMessageDef(string name)
		{
            MessageDef message = (MessageDef)CacheManager.Get(CacheRegions.MessageByName, name);
            if (message == null)
            {
                message = MessageDao.Retrieve(name);

                if (message != null)
                {
                    CacheManager.Update(CacheRegions.MessageByName, message.Name, message);
                    CacheManager.Update(CacheRegions.MessageById, message.Id, message);
                }
            }

            return message;
		}

		public int HowManyMessageDefs(List<Dictionary<string, object>> parms)
		{
			return MessageDao.HowManyMessages(parms);
		}

		public List<MessageDef> GetAllMessageDefs(LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null) batchInfo.Validate();
			var messages = MessageDao.RetrieveAll(batchInfo) ?? new List<MessageDef>();
            foreach (MessageDef message in messages)
            {
                CacheManager.Update(CacheRegions.MessageByName, message.Name, message);
                CacheManager.Update(CacheRegions.MessageById, message.Id, message);
            }

            return messages;
		}

		public List<MessageDef> GetMessageDefs(long[] ids, bool populateAttributes)
		{
			var messages = MessageDao.Retrieve(ids, populateAttributes) ?? new List<MessageDef>();
            foreach (MessageDef message in messages)
            {
                CacheManager.Update(CacheRegions.MessageByName, message.Name, message);
                CacheManager.Update(CacheRegions.MessageById, message.Id, message);
            }

            return messages;
        }

		public List<MessageDef> GetAllChangedMessageDefs(DateTime since)
		{
			var messages = MessageDao.RetrieveChangedObjects(since) ?? new List<MessageDef>();
            foreach (MessageDef message in messages)
            {
                CacheManager.Update(CacheRegions.MessageByName, message.Name, message);
                CacheManager.Update(CacheRegions.MessageById, message.Id, message);
            }

            return messages;
        }

		public List<long> GetMessageDefIds(bool activeOnly, string sortExpression, bool ascending, long? folderId = null)
		{
			List<Dictionary<string, object>> parmsList = new List<Dictionary<string, object>>();
			if (activeOnly)
			{
				Dictionary<string, object> parms = new Dictionary<string, object>();
				parms.Add("Property", "Active");
				parms.Add("Predicate", LWCriterion.Predicate.Eq);
				parms.Add("Value", true);
				parmsList.Add(parms);
			}
			return MessageDao.RetrieveMessageDefIds(parmsList, sortExpression, ascending, folderId);
		}

		public List<long> GetMessageDefIds(List<Dictionary<string, object>> parms, string sortExpression, bool ascending, long? folderId = null)
		{
			return MessageDao.RetrieveMessageDefIds(parms, sortExpression, ascending, folderId);
		}

		public List<MessageDef> GetMessageDefs(List<Dictionary<string, object>> parms, bool populateAttributes, LWQueryBatchInfo batchInfo)
		{
			if (batchInfo != null) batchInfo.Validate();
			var messages = MessageDao.RetrieveMessageDefs(parms, populateAttributes, batchInfo) ?? new List<MessageDef>();
            foreach (MessageDef message in messages)
            {
                CacheManager.Update(CacheRegions.MessageByName, message.Name, message);
                CacheManager.Update(CacheRegions.MessageById, message.Id, message);
            }

            return messages;
        }

		public void DeleteMessageDef(long id)
		{
			_logger.Trace(_className, "DeleteMessageDef", "Deleting customer message with id = " + id);
            MessageDef message = MessageDao.Retrieve(id);
            if (message != null)
            {
                MessageDao.Delete(id);
                CacheManager.Remove(CacheRegions.MessageByName, message);
                CacheManager.Remove(CacheRegions.MessageById, message);
            }
		}

        #endregion

        #region Category Management

        public void AddNotificationCategory(NotificationCategory notificationCategory)
        {
            _logger.Trace(_className, "AddNotificationCategory", "Creating new notification category " + notificationCategory.Name);
            NotificationCategoryDao.Create(notificationCategory);
        }

        public void UpdateNotificationCategory(NotificationCategory category)
        {
            NotificationCategoryDao.Update(category);
        }

        public List<NotificationCategory> GetNotificationCategories()
        {
            return NotificationCategoryDao.RetrieveAll() ?? new List<NotificationCategory>();
        }

        public List<NotificationCategory> GetNotificationCategories(bool isVisibleInLN)
        {
            return NotificationCategoryDao.RetrieveByVisibility(isVisibleInLN) ?? new List<NotificationCategory>();
        }

        public List<NotificationCategory> GetNotificationCategories(long[] ids)
        {
            return NotificationCategoryDao.RetrieveNotificationCategoriesByIds(ids) ?? new List<NotificationCategory>();
        }

        public List<NotificationCategory> GetTopLevelNotificationCategories(bool isVisibleInLN)
        {
            return NotificationCategoryDao.RetrieveChildNotificationCategoriesAll(0L, isVisibleInLN) ?? new List<NotificationCategory>();
        }

        public List<NotificationCategory> GetTopLevelNotificationCategoriesByType(CategoryType type, bool isVisibleInLN)
        {
            return NotificationCategoryDao.RetrieveTopLevelNotificationCategoriesByType(type, isVisibleInLN) ?? new List<NotificationCategory>();
        }

        public List<NotificationCategory> GetChildNotificationCategories(long categoryId, bool isVisibleInLN)
        {
            return GetChildNotificationCategories(categoryId, isVisibleInLN, null);
        }

        public List<NotificationCategory> GetChildNotificationCategories(long categoryId, bool isVisibleInLN, LWQueryBatchInfo batchInfo)
        {
            if (batchInfo != null) batchInfo.Validate();
            return NotificationCategoryDao.RetrieveChildNotificationCategoriesAll(categoryId, isVisibleInLN, batchInfo) ?? new List<NotificationCategory>();
        }

        public List<NotificationCategory> GetAllChangedNotificationCategories(DateTime since)
        {
            return NotificationCategoryDao.RetrieveChangedObjects(since) ?? new List<NotificationCategory>();
        }

        public NotificationCategory GetNotificationCategory(long categoryId)
        {
            return NotificationCategoryDao.Retrieve(categoryId);
        }

        public NotificationCategory GetNotificationCategory(long parentId, string catName)
        {
            return NotificationCategoryDao.Retrieve(parentId, catName);
        }

        public List<NotificationCategory> GetNotificationCategoriesByType(CategoryType type, bool isVisibleInLN)
        {
            return NotificationCategoryDao.RetrieveByType(type, isVisibleInLN) ?? new List<NotificationCategory>();
        }

        public void DeleteNotificationCategory(long categoryId)
        {
            _logger.Trace(_className, "DeleteCategory", "Deleting category with id = " + categoryId);
            NotificationCategoryDao.Delete(categoryId);
        }

        #endregion

        #region Push Notification Definition

        public void CreateNotificationDef(NotificationDef notification)
        {
            _logger.Trace(_className, "CreateNotificationDef", "Creating new notification " + notification.Name);
            NotificationDao.Create(notification);
        }

        public void UpdateNotificationDef(NotificationDef notification)
        {
            NotificationDao.Update(notification);
        }

        public NotificationDef GetNotificationDef(long id)
        {
            return NotificationDao.Retrieve(id);
        }

        public NotificationDef GetNotificationDef(string name)
        {
            return NotificationDao.Retrieve(name);
        }

        public int HowManyNotificationDefs(List<Dictionary<string, object>> parms)
        {
            return NotificationDao.HowManyNotifications(parms);
        }

        public List<NotificationDef> GetAllNotificationDefs(LWQueryBatchInfo batchInfo)
        {
            if (batchInfo != null) batchInfo.Validate();
            return NotificationDao.RetrieveAll(batchInfo) ?? new List<NotificationDef>();
        }

        public List<NotificationDef> GetNotificationDefs(long[] ids, bool populateAttributes)
        {
            return NotificationDao.Retrieve(ids, populateAttributes) ?? new List<NotificationDef>();
        }

        public List<NotificationDef> GetAllChangedNotificationDefs(DateTime since)
        {
            return NotificationDao.RetrieveChangedObjects(since) ?? new List<NotificationDef>();
        }

        public List<long> GetNotificationDefIds(bool activeOnly, string sortExpression, bool ascending, long? folderId = null)
        {
            List<Dictionary<string, object>> parmsList = new List<Dictionary<string, object>>();
            if (activeOnly)
            {
                Dictionary<string, object> parms = new Dictionary<string, object>();
                parms.Add("Property", "Active");
                parms.Add("Predicate", LWCriterion.Predicate.Eq);
                parms.Add("Value", true);
                parmsList.Add(parms);
            }
            return NotificationDao.RetrieveNotificationDefIds(parmsList, sortExpression, ascending, folderId);
        }

        public List<long> GetNotificationDefIds(List<Dictionary<string, object>> parms, string sortExpression, bool ascending, long? folderId = null)
        {
            return NotificationDao.RetrieveNotificationDefIds(parms, sortExpression, ascending, folderId);
        }

        public List<NotificationDef> GetNotificationDefs(List<Dictionary<string, object>> parms, bool populateAttributes, LWQueryBatchInfo batchInfo)
        {
            if (batchInfo != null) batchInfo.Validate();
            return NotificationDao.RetrieveNotificationDefs(parms, populateAttributes, batchInfo) ?? new List<NotificationDef>();
        }

        public void DeleteNotificationDef(long id)
        {
            _logger.Trace(_className, "DeleteNotificationDef", "Deleting notification with id = " + id);
            NotificationDao.Delete(id);
        }

        #endregion

        #region Attribute Definition

        public void CreateContentAttributeDef(ContentAttributeDef att)
		{
			ContentAttributeDefDao.Create(att);
			CacheManager.Update(CacheRegions.ContentAttributeDefByName, att.Name, att);
            CacheManager.Update(CacheRegions.ContentAttributeDefById, att.ID, att);
		}

		public void UpdateContentAttributeDef(ContentAttributeDef att)
		{
			ContentAttributeDefDao.Update(att);
			CacheManager.Update(CacheRegions.ContentAttributeDefByName, att.Name, att);
            CacheManager.Update(CacheRegions.ContentAttributeDefById, att.ID, att);
        }

		public ContentAttributeDef GetContentAttributeDef(long attId)
		{
            ContentAttributeDef att = (ContentAttributeDef)CacheManager.Get(CacheRegions.ContentAttributeDefById, attId);
			if (att == null)
			{
				att = ContentAttributeDefDao.Retrieve(attId);
				if (att != null)
				{
					CacheManager.Update(CacheRegions.ContentAttributeDefByName, att.Name, att);
                    CacheManager.Update(CacheRegions.ContentAttributeDefById, att.ID, att);
                }
			}
			return att;
		}

		public ContentAttributeDef GetContentAttributeDef(string attName)
		{
			ContentAttributeDef att = (ContentAttributeDef)CacheManager.Get(CacheRegions.ContentAttributeDefByName, attName);
			if (att == null)
			{
				att = ContentAttributeDefDao.Retrieve(attName);
				if (att != null)
				{
					CacheManager.Update(CacheRegions.ContentAttributeDefByName, att.Name, att);
                    CacheManager.Update(CacheRegions.ContentAttributeDefById, att.ID, att);
                }
			}
			return att;
		}

		public List<ContentAttributeDef> GetAllChangedContentAttributeDefs(DateTime since)
		{
			return ContentAttributeDefDao.RetrieveChangedObjects(since) ?? new List<ContentAttributeDef>();
		}

		public List<ContentAttributeDef> GetAllContentAttributeDefs()
		{
			var list = ContentAttributeDefDao.RetrieveAll() ?? new List<ContentAttributeDef>();
			foreach (ContentAttributeDef att in list)
			{
				CacheManager.Update(CacheRegions.ContentAttributeDefByName, att.Name, att);
                CacheManager.Update(CacheRegions.ContentAttributeDefById, att.ID, att);
            }
			return list;
		}

		public List<ContentAttributeDef> GetAllContentAttributeDefs(ContentObjType contentType)
		{
			var list = ContentAttributeDefDao.RetrieveAll(contentType) ?? new List<ContentAttributeDef>();
			foreach (ContentAttributeDef att in list)
			{
				CacheManager.Update(CacheRegions.ContentAttributeDefByName, att.Name, att);
                CacheManager.Update(CacheRegions.ContentAttributeDefById, att.ID, att);
            }
			return list;
		}

		public void DeleteContentAttributeDef(long attId)
		{
			ContentAttributeDef att = GetContentAttributeDef(attId);
			if (att != null)
			{
				ContentAttributeDefDao.Delete(attId);
				CacheManager.Remove(CacheRegions.ContentAttributeDefByName, att.Name);
                CacheManager.Remove(CacheRegions.ContentAttributeDefById, att.ID);
            }
		}

        #endregion


        protected static bool DuplicateProductPartNumberAllowed()
		{
			// check by part number
			bool allowDuplicatePartNmbr = false;
			string strDupPartNmber = LWConfigurationUtil.GetConfigurationValue("AllowDuplicateProductPartNumbers");
			if (!string.IsNullOrEmpty(strDupPartNmber))
			{
				try
				{
					allowDuplicatePartNmbr = bool.Parse(strDupPartNmber);
				}
				catch (Exception)
				{
					allowDuplicatePartNmbr = false;
				}
			}
			return allowDuplicatePartNmbr;
		}

		private static List<int> ParseCertNumberFormat(string certNumberFormat, ref string outputCertNumberFormat)
		{
			List<int> result = new List<int>();
			outputCertNumberFormat = certNumberFormat;
			string tmp = certNumberFormat;
			while (!string.IsNullOrEmpty(tmp) && tmp.Contains('#'))
			{
				int start = 0;
				int end = 0;
				int nChars = StringUtils.CountConsecutiveCharacters(tmp, '#', ref start, ref end);
				result.Add(nChars);
				tmp = StringUtils.Replace(tmp, "{" + (result.Count - 1).ToString() + "}", start, end);
			}
			outputCertNumberFormat = tmp;
			return result;
		}

		private static string GeneratePromoCertNumber(List<int> buckets, Random random, string outputCertNumberFormat)
		{
			string result = outputCertNumberFormat;
			for (int i = 0; i < buckets.Count; i++)
			{
				int numChars = buckets[i];
				double randVal = random.NextDouble();
				long val = Convert.ToInt64(Math.Floor(randVal * Math.Pow(10.0, (double)numChars)));
				result = result.Replace("{" + i.ToString() + "}", val.ToString("D" + numChars));
			}
			return result;
		}

		private string MakeKey(long id1, long id2)
		{
			return id1.ToString() + " ::" + id2.ToString();
		}

		private void UpdateProductsCache(Product product)
		{
			InvalidateProductsCache();
		}

		private void InvalidateProductsCache()
		{
			// invalidate product list cache
			List<Product> list = (List<Product>)CacheManager.Get(CacheRegions.Products, CacheRegions.Products);
			if (list != null && list.Count > 0)
			{
				// should really update or add this product to the list
				CacheManager.Remove(CacheRegions.Products, CacheRegions.Products);
			}
		}

		private DataTable CreateRowDataTable(long elementID)
		{
			DataTable result = new DataTable();

			List<StructuredContentAttribute> globalAttributes = GetGlobalAttributes();
			foreach (StructuredContentAttribute globalAttribute in globalAttributes)
			{
				result.Columns.Add(globalAttribute.Name);
			}

			List<StructuredContentAttribute> elementAttributes = GetElementAttributes(elementID);
			foreach (StructuredContentAttribute elementAttribute in elementAttributes)
			{
				result.Columns.Add(elementAttribute.Name);
			}
			return result;
		}

        /// <summary>
        /// Creates a new exchange record 
        /// </summary>
        /// <param name="exchangeRate"></param>
        public void CreateExchangeRate(ExchangeRate exchangeRate)
        {
            ///If the from currency is not passed we should always default to the config
            if (string.IsNullOrWhiteSpace(exchangeRate.FromCurrency))
            {
                exchangeRate.FromCurrency = LWConfigurationUtil.GetConfigurationValue("LoyaltyCurrencyAsPayment DefaultCurrency");
	}

            //We need to verify if the entry already exists
            ExchangeRate existingRate = ExchangeRateDao.Retrive(exchangeRate.FromCurrency, exchangeRate.ToCurrency);
            
            //Perform some validations
            ValidateExchangeRate(exchangeRate);

            if (existingRate != null)
            {
                throw new LWException("Exchange rate already exists.");
            }

            ExchangeRateDao.Create(exchangeRate);

        }

        /// <summary>
        /// Update exchange rate record
        /// </summary>
        /// <param name="exchangeRate"></param>
        public void UpdateExchangeRate(ExchangeRate exchangeRate)
        {

            //If the FromCurrency is not passed we need to get the default
            if (string.IsNullOrWhiteSpace(exchangeRate.FromCurrency))
            {
                exchangeRate.FromCurrency = LWConfigurationUtil.GetConfigurationValue("LoyaltyCurrencyAsPayment DefaultCurrency");
            }

            ExchangeRate existingRate = ExchangeRateDao.Retrive(exchangeRate.FromCurrency, exchangeRate.ToCurrency);

            if (existingRate != null && existingRate.Id != existingRate.Id)
            {
                throw new LWException("Unable to update exchange rate. An exchange rate already exists with those values.");
            }

            //Peform some validations
            ValidateExchangeRate(exchangeRate);

            ExchangeRateDao.Update(exchangeRate);
        }

        public void DeleteExchangeRate(long exchangeRateId)
        {
            ExchangeRateDao.Delete(exchangeRateId);
        }

        public ExchangeRate GetExchangeRateById(long exchangeRateId)
        {
            return ExchangeRateDao.Retrive(exchangeRateId);
        }

        public ExchangeRate GetExchangeRate(string fromCurrency, string toCurrency)
        {
            return ExchangeRateDao.Retrive(fromCurrency, toCurrency);
        }

        public PetaPoco.Page<ExchangeRate> GetAllExchangeRates(long pageNumber, long resultsPerPage)
        {
            return ExchangeRateDao.RetrieveAll(pageNumber, resultsPerPage);
        }

        public List<ExchangeRate> GetAllExchangeRates()
        {
            return GetAllExchangeRates(1, long.MaxValue).Items;
        }

        public PetaPoco.Page<ExchangeRate> GetExchangeRatesByFromCurrency(string fromCurrency, long pageNumber, long resultsPerPage)
        {
            return ExchangeRateDao.RetrieveAllByFromCurrency(fromCurrency, pageNumber, resultsPerPage);
        }

        public List<ExchangeRate> GetExchangeRatesByFromCurrency(string fromCurrency)
        {
            return GetExchangeRatesByFromCurrency(fromCurrency, 1, long.MaxValue).Items;
        }

        public PetaPoco.Page<ExchangeRate> GetExchangeRatesByToCurrency(string toCurrency, long pageNumber, long resultsPerPage)
        {
            return ExchangeRateDao.RetrieveAllByToCurrency(toCurrency, pageNumber, resultsPerPage);
        }

        public List<ExchangeRate> GetExchangeRatesByToCurrency(string toCurrency)
        {
            return GetExchangeRatesByToCurrency(toCurrency, 1, long.MaxValue).Items;
        }

        public PetaPoco.Page<ExchangeRate> GetExchangeRatesByProperties(List<Dictionary<string, object>> properties, long pageNumber, long resultsPerPage)
        {
            return ExchangeRateDao.RetriveByProperty(properties, pageNumber, resultsPerPage);
        }

        public List<ExchangeRate> GetExchangeRatesByProperties(List<Dictionary<string, object>> properties)
        {
            return GetExchangeRatesByProperties(properties, 1, long.MaxValue).Items;
        }

        /// <summary>
        /// validates some basic exchange rate cases
        /// </summary>
        /// <param name="exchangeRate"></param>
        private void ValidateExchangeRate(ExchangeRate exchangeRate)
        {
            if (string.IsNullOrWhiteSpace(exchangeRate.ToCurrency))
            {
                throw new LWException("ToCurrency is a required property.  Please provide a ToCurrency code.");
            }

            if (exchangeRate.Rate < 0)
            {
                throw new LWException("Invalid exchange rate. The rate property must be a positive number.");
            }
        }

        
	}
}
