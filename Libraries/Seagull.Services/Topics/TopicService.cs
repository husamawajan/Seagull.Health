using System;
using System.Collections.Generic;
using System.Linq;
using Seagull.Core;
using Seagull.Core.Caching;
using Seagull.Core.Data;
using Seagull.Core.Domain.Security;
using Seagull.Core.Domain.Stores;
using Seagull.Core.Domain.Topics;
using Seagull.Services.Users;
using Seagull.Services.Events;
using Seagull.Services.Stores;

namespace Seagull.Services.Topics
{
    /// <summary>
    /// Topic service
    /// </summary>
    public partial class TopicService : ITopicService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : ignore ACL?
        /// {2} : show hidden?
        /// </remarks>
        private const string TOPICS_ALL_KEY = "Seagull.topics.all-{0}-{1}-{2}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : topic ID
        /// </remarks>
        private const string TOPICS_BY_ID_KEY = "Seagull.topics.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string TOPICS_PATTERN_KEY = "Seagull.topics.";

        #endregion
        
        #region Fields

        private readonly IRepository<Topic> _topicRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public TopicService(IRepository<Topic> topicRepository, 
            IRepository<StoreMapping> storeMappingRepository,
            IStoreMappingService storeMappingService,
            IWorkContext workContext,
            IRepository<AclRecord> aclRepository,
            IEventPublisher eventPublisher,
            ICacheManager cacheManager)
        {
            this._topicRepository = topicRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._storeMappingService = storeMappingService;
            this._workContext = workContext;
            this._aclRepository = aclRepository;
            this._eventPublisher = eventPublisher;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        public virtual void DeleteTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            _topicRepository.Delete(topic);

            //cache
            _cacheManager.RemoveByPattern(TOPICS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(topic);
        }

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="topicId">The topic identifier</param>
        /// <returns>Topic</returns>
        public virtual Topic GetTopicById(int topicId)
        {
            if (topicId == 0)
                return null;

            string key = string.Format(TOPICS_BY_ID_KEY, topicId);
            return _cacheManager.Get(key, () => _topicRepository.GetById(topicId));
        }

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="systemName">The topic system name</param>
        /// <param name="storeId">Store identifier; pass 0 to ignore filtering by store and load the first one</param>
        /// <returns>Topic</returns>
        public virtual Topic GetTopicBySystemName(string systemName, int storeId = 0)
        {
            if (String.IsNullOrEmpty(systemName))
                return null;

            var query = _topicRepository.Table;
            query = query.Where(t => t.SystemName == systemName);
            query = query.OrderBy(t => t.Id);
            var topics = query.ToList();
            if (storeId > 0)
            {
                topics = topics.Where(x => _storeMappingService.Authorize(x, storeId)).ToList();
            }
            return topics.FirstOrDefault();
        }

        /// <summary>
        /// Gets all topics
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="ignorAcl">A value indicating whether to ignore ACL rules</param>
        /// <param name="showHidden">A value indicating whether to show hidden topics</param>
        /// <returns>Topics</returns>
        public virtual IList<Topic> GetAllTopics(int storeId, bool ignorAcl = false, bool showHidden = false)
        {
            string key = string.Format(TOPICS_ALL_KEY, storeId, ignorAcl, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = _topicRepository.Table;
                query = query.OrderBy(t => t.DisplayOrder).ThenBy(t => t.SystemName);

                if (!showHidden)
                    query = query.Where(t => t.Published);

                return query.ToList();                            
            });
        }

        /// <summary>
        /// Inserts a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        public virtual void InsertTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            _topicRepository.Insert(topic);

            //cache
            _cacheManager.RemoveByPattern(TOPICS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(topic);
        }

        /// <summary>
        /// Updates the topic
        /// </summary>
        /// <param name="topic">Topic</param>
        public virtual void UpdateTopic(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            _topicRepository.Update(topic);

            //cache
            _cacheManager.RemoveByPattern(TOPICS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(topic);
        }

        #endregion
    }
}
