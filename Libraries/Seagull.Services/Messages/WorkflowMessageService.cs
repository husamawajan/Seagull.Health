using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Seagull.Core;
using Seagull.Core.Domain.Common;
using Seagull.Core.Domain.Users;
using Seagull.Core.Domain.Messages;
using Seagull.Services.Users;
using Seagull.Services.Events;
using Seagull.Services.Localization;
using Seagull.Services.Stores;

namespace Seagull.Services.Messages
{
    public partial class WorkflowMessageService : IWorkflowMessageService
    {
        #region Fields

        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ILanguageService _languageService;
        private readonly ITokenizer _tokenizer;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly CommonSettings _commonSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly HttpContextBase _httpContext;

        #endregion

        #region Ctor

        public WorkflowMessageService(IMessageTemplateService messageTemplateService,
            IQueuedEmailService queuedEmailService,
            ILanguageService languageService,
            ITokenizer tokenizer, 
            IEmailAccountService emailAccountService,
            IMessageTokenProvider messageTokenProvider,
            IStoreService storeService,
            IStoreContext storeContext,
            CommonSettings commonSettings,
            EmailAccountSettings emailAccountSettings,
            IEventPublisher eventPublisher,
            HttpContextBase httpContext)
        {
            this._messageTemplateService = messageTemplateService;
            this._queuedEmailService = queuedEmailService;
            this._languageService = languageService;
            this._tokenizer = tokenizer;
            this._emailAccountService = emailAccountService;
            this._messageTokenProvider = messageTokenProvider;
            this._storeService = storeService;
            this._storeContext = storeContext;
            this._commonSettings = commonSettings;
            this._emailAccountSettings = emailAccountSettings;
            this._eventPublisher = eventPublisher;
            this._httpContext = httpContext;
        }

        #endregion

        #region Utilities
        
        protected virtual MessageTemplate GetActiveMessageTemplate(string messageTemplateName, int storeId)
        {
            var messageTemplate = _messageTemplateService.GetMessageTemplateByName(messageTemplateName, storeId);

            //no template found
            if (messageTemplate == null)
                return null;

            //ensure it's active
            var isActive = messageTemplate.IsActive;
            if (!isActive)
                return null;

            return messageTemplate;
        }

        protected virtual EmailAccount GetEmailAccountOfMessageTemplate(MessageTemplate messageTemplate, int languageId)
        {
            var emailAccountId = messageTemplate.GetLocalized(mt => mt.EmailAccountId, languageId);
            //some 0 validation (for localizable "Email account" dropdownlist which saves 0 if "Standard" value is chosen)
            if (emailAccountId == 0)
                emailAccountId = messageTemplate.EmailAccountId;

            var emailAccount = _emailAccountService.GetEmailAccountById(emailAccountId);
            if (emailAccount == null)
                emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            if (emailAccount == null)
                emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
            return emailAccount;

        }

        protected virtual int EnsureLanguageIsActive(int languageId, int storeId)
        {
            //load language by specified ID
            var language = _languageService.GetLanguageById(languageId);

            if (language == null || !language.Published)
            {
                //load any language from the specified store
                language = _languageService.GetAllLanguages(storeId: storeId).FirstOrDefault();
            }
            if (language == null || !language.Published)
            {
                //load any language
                language = _languageService.GetAllLanguages().FirstOrDefault();
            }

            if (language == null)
                throw new Exception("No active language could be loaded");
            return language.Id;
        }

        #endregion

        #region Methods

        #region User workflow

        /// <summary>
        /// Sends 'New user' notification message to a store owner
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendUserRegisteredNotificationMessage(User user, int languageId)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.UserRegisteredNotification, store.Id);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddUserTokens(tokens, user);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }

        /// <summary>
        /// Sends a welcome message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendUserWelcomeMessage(User user, int languageId)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.UserWelcomeMessage, store.Id);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddUserTokens(tokens, user);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var toEmail = user.Email;
            var toName = user.GetFullName();

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }

        /// <summary>
        /// Sends an email validation message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendUserEmailValidationMessage(User user, int languageId)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.UserEmailValidationMessage, store.Id);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddUserTokens(tokens, user);


            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var toEmail = user.Email;
            var toName = user.GetFullName();
                
            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }

        /// <summary>
        /// Sends an email re-validation message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendUserEmailRevalidationMessage(User user, int languageId)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.UserEmailRevalidationMessage, store.Id);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddUserTokens(tokens, user);


            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            //email to re-validate
            var toEmail = user.EmailToRevalidate;
            var toName = user.GetFullName();

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }

        /// <summary>
        /// Sends password recovery message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendUserPasswordRecoveryMessage(User user, int languageId)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.UserPasswordRecoveryMessage, store.Id);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddUserTokens(tokens, user);


            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var toEmail = user.Email;
            var toName = user.GetFullName();

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }

        #endregion

        

        
        
        #region Send a message to a friend


        /// <summary>
        /// Sends wishlist "email a friend" message
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="userEmail">User's email</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendWishlistEmailAFriendMessage(User user, int languageId,
             string userEmail, string friendsEmail, string personalMessage)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.WishlistToFriendMessage, store.Id);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddUserTokens(tokens, user);
            tokens.Add(new Token("Wishlist.PersonalMessage", personalMessage, true));
            tokens.Add(new Token("Wishlist.Email", userEmail));

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, friendsEmail, string.Empty);
        }

        #endregion

        

        

        #region Misc

       

        /// <summary>
        /// Sends a "new VAT submitted" notification to a store owner
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="vatName">Received VAT name</param>
        /// <param name="vatAddress">Received VAT address</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNewVatSubmittedStoreOwnerNotification(User user,
            string vatName, string vatAddress, int languageId)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.NewVatSubmittedStoreOwnerNotification, store.Id);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            _messageTokenProvider.AddUserTokens(tokens, user);
            tokens.Add(new Token("VatValidationResult.Name", vatName));
            tokens.Add(new Token("VatValidationResult.Address", vatAddress));

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
        }

        /// <summary>
        /// Sends "contact us" message
        /// </summary>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="subject">Email subject. Pass null if you want a message template subject to be used.</param>
        /// <param name="body">Email body</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendContactUsMessage(int languageId, string senderEmail,
            string senderName, string subject, string body)
        {
            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplate = GetActiveMessageTemplate(MessageTemplateSystemNames.ContactUsMessage, store.Id);
            if (messageTemplate == null)
                return 0;

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            string fromEmail;
            string fromName;
            //required for some SMTP servers
            if (_commonSettings.UseSystemEmailForContactUsForm)
            {
                fromEmail = emailAccount.Email;
                fromName = emailAccount.DisplayName;
                body = string.Format("<strong>From</strong>: {0} - {1}<br /><br />{2}",
                    _httpContext.Server.HtmlEncode(senderName), _httpContext.Server.HtmlEncode(senderEmail), body);
            }
            else
            {
                fromEmail = senderEmail;
                fromName = senderName;
            }

            //tokens
            var tokens = new List<Token>();
            _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);
            tokens.Add(new Token("ContactUs.SenderEmail", senderEmail));
            tokens.Add(new Token("ContactUs.SenderName", senderName));
            tokens.Add(new Token("ContactUs.Body", body, true));

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            var toEmail = emailAccount.Email;
            var toName = emailAccount.DisplayName;

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                fromEmail: fromEmail,
                fromName: fromName,
                subject: subject,
                replyToEmailAddress: senderEmail,
                replyToName: senderName);
        }

        
        /// <summary>
        /// Sends a test email
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <param name="sendToEmail">Send to email</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendTestEmail(int messageTemplateId, string sendToEmail, List<Token> tokens, int languageId)
        {
            var messageTemplate = _messageTemplateService.GetMessageTemplateById(messageTemplateId);
            if (messageTemplate == null)
                throw new ArgumentException("Template cannot be loaded");

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, sendToEmail, null);
        }

        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="toEmailAddress">Recipient email address</param>
        /// <param name="toName">Recipient name</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name</param>
        /// <param name="replyToEmailAddress">"Reply to" email</param>
        /// <param name="replyToName">"Reply to" name</param>
        /// <param name="fromEmail">Sender email. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="fromName">Sender name. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="subject">Subject. If specified, then it overrides subject of a message template</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNotification(MessageTemplate messageTemplate,
            EmailAccount emailAccount, int languageId, IEnumerable<Token> tokens,
            string toEmailAddress, string toName,
            string attachmentFilePath = null, string attachmentFileName = null,
            string replyToEmailAddress = null, string replyToName = null,
            string fromEmail = null, string fromName = null, string subject = null)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException("messageTemplate");

            if (emailAccount == null)
                throw new ArgumentNullException("emailAccount");

            //retrieve localized message template data
            var bcc = messageTemplate.GetLocalized(mt => mt.BccEmailAddresses, languageId);
            if (String.IsNullOrEmpty(subject))
                subject = messageTemplate.GetLocalized(mt => mt.Subject, languageId);
            var body = messageTemplate.GetLocalized(mt => mt.Body, languageId);

            //Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            //limit name length
            toName = CommonHelper.EnsureMaximumLength(toName, 300);

            var email = new QueuedEmail
            {
                Priority = QueuedEmailPriority.High,
                From = !string.IsNullOrEmpty(fromEmail) ? fromEmail : emailAccount.Email,
                FromName = !string.IsNullOrEmpty(fromName) ? fromName : emailAccount.DisplayName,
                To = toEmailAddress,
                ToName = toName,
                ReplyTo = replyToEmailAddress,
                ReplyToName = replyToName,
                CC = string.Empty,
                Bcc = bcc,
                Subject = subjectReplaced,
                Body = bodyReplaced,
                AttachmentFilePath = attachmentFilePath,
                AttachmentFileName = attachmentFileName,
                AttachedDownloadId = messageTemplate.AttachedDownloadId,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id,
                DontSendBeforeDateUtc = !messageTemplate.DelayBeforeSend.HasValue ? null
                    : (DateTime?)(DateTime.UtcNow + TimeSpan.FromHours(messageTemplate.DelayPeriod.ToHours(messageTemplate.DelayBeforeSend.Value)))
            };

            _queuedEmailService.InsertQueuedEmail(email);
            return email.Id;
        }

        #endregion

        #endregion
    }
}
