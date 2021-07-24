using System.Web.Mvc;
using System.Web.Routing;
using Seagull.Web.Framework.Localization;
using Seagull.Web.Framework.Mvc.Routes;

namespace Seagull.Web.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //We reordered our routes so the most used ones are on top. It can improve performance.

            //home page
            routes.MapLocalizedRoute("HomePage",
                            "Admin",
                            new { controller = "Home", action = "Index" },
                            new[] { "Seagull.Admin.Controllers" });

            //login
            routes.MapLocalizedRoute("Login",
                            "login/",
                            new { controller = "User", action = "Login" },
                            new[] { "Seagull.Web.Controllers" });
            //register
            routes.MapLocalizedRoute("Register",
                            "register/",
                            new { controller = "User", action = "Register" },
                            new[] { "Seagull.Web.Controllers" });
            //logout
            routes.MapLocalizedRoute("Logout",
                            "logout/",
                            new { controller = "User", action = "Logout" },
                            new[] { "Seagull.Web.Controllers" });

            //shopping cart
            routes.MapLocalizedRoute("ShoppingCart",
                            "cart/",
                            new { controller = "ShoppingCart", action = "Cart" },
                            new[] { "Seagull.Web.Controllers" });
            ////estimate shipping
            //routes.MapLocalizedRoute("EstimateShipping",
            //                "cart/estimateshipping",
            //                new {controller = "ShoppingCart", action = "GetEstimateShipping"},
            //                new[] {"Seagull.Web.Controllers"});
            ////wishlist
            //routes.MapLocalizedRoute("Wishlist",
            //                "wishlist/{userGuid}",
            //                new { controller = "ShoppingCart", action = "Wishlist", userGuid = UrlParameter.Optional },
            //                new[] { "Seagull.Web.Controllers" });

            //user account links
            routes.MapLocalizedRoute("UserInfo",
                            "user/info",
                            new { controller = "User", action = "Info" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("UserAddresses",
                            "user/addresses",
                            new { controller = "User", action = "Addresses" },
                            new[] { "Seagull.Web.Controllers" });
            

            //contact us
            routes.MapLocalizedRoute("ContactUs",
                            "contactus",
                            new { controller = "Common", action = "ContactUs" },
                            new[] { "Seagull.Web.Controllers" });
            //sitemap
            routes.MapLocalizedRoute("Sitemap",
                            "sitemap",
                            new { controller = "Common", action = "Sitemap" },
                            new[] { "Seagull.Web.Controllers" });

            
            //change language (AJAX link)
            routes.MapLocalizedRoute("ChangeLanguage",
                            "changelanguage/{langid}",
                            new { controller = "Common", action = "SetLanguage" },
                            new { langid = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            //change tax (AJAX link)
            routes.MapLocalizedRoute("ChangeTaxType",
                            "changetaxtype/{usertaxtype}",
                            new { controller = "Common", action = "SetTaxType" },
                            new { usertaxtype = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });

            
            //downloads
            routes.MapRoute("GetSampleDownload",
                            "download/sample/{productid}",
                            new { controller = "Download", action = "Sample" },
                            new { productid = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });



            

            //login page for checkout as guest
            routes.MapLocalizedRoute("LoginCheckoutAsGuest",
                            "login/checkoutasguest",
                            new { controller = "User", action = "Login", checkoutAsGuest = true },
                            new[] { "Seagull.Web.Controllers" });
            //register result page
            routes.MapLocalizedRoute("RegisterResult",
                            "registerresult/{resultId}",
                            new { controller = "User", action = "RegisterResult" },
                            new { resultId = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            //check username availability
            routes.MapLocalizedRoute("CheckUsernameAvailability",
                            "user/checkusernameavailability",
                            new { controller = "User", action = "CheckUsernameAvailability" },
                            new[] { "Seagull.Web.Controllers" });

            //passwordrecovery
            routes.MapLocalizedRoute("PasswordRecovery",
                            "passwordrecovery",
                            new { controller = "User", action = "PasswordRecovery" },
                            new[] { "Seagull.Web.Controllers" });
            //password recovery confirmation
            routes.MapLocalizedRoute("PasswordRecoveryConfirm",
                            "passwordrecovery/confirm",
                            new { controller = "User", action = "PasswordRecoveryConfirm" },                            
                            new[] { "Seagull.Web.Controllers" });

            //routes.MapLocalizedRoute("UserChangePassword",
            //                "user/changepassword",
            //                new { controller = "User", action = "ChangePassword" },
            //                new[] { "Seagull.Web.Controllers" });

            //Change Password
            routes.MapLocalizedRoute("UserChangePassword",
                            "Admin/ChangePassword/ChangePassword",
                            new { controller = "ChangePassword", action = "ChangePassword" },
                            new[] { "Seagull.Admin.Controllers" });
            routes.MapLocalizedRoute("UserAvatar",
                            "user/avatar",
                            new { controller = "User", action = "Avatar" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("AccountActivation",
                            "user/activation",
                            new { controller = "User", action = "AccountActivation" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("EmailRevalidation",
                            "user/revalidateemail",
                            new { controller = "User", action = "EmailRevalidation" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("UserForumSubscriptions",
                            "boards/forumsubscriptions",
                            new { controller = "Boards", action = "UserForumSubscriptions" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("UserForumSubscriptionsPaged",
                            "boards/forumsubscriptions/{page}",
                            new { controller = "Boards", action = "UserForumSubscriptions", page = UrlParameter.Optional },
                            new { page = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("UserAddressEdit",
                            "user/addressedit/{addressId}",
                            new { controller = "User", action = "AddressEdit" },
                            new { addressId = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("UserAddressAdd",
                            "user/addressadd",
                            new { controller = "User", action = "AddressAdd" },
                            new[] { "Seagull.Web.Controllers" });
            //user profile page
            routes.MapLocalizedRoute("UserProfile",
                            "profile/{id}",
                            new { controller = "Profile", action = "Index" },
                            new { id = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("UserProfilePaged",
                            "profile/{id}/page/{page}",
                            new { controller = "Profile", action = "Index" },
                            new { id = @"\d+", page = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });

            //orders
            routes.MapLocalizedRoute("OrderDetails",
                            "orderdetails/{orderId}",
                            new { controller = "Order", action = "Details" },
                            new { orderId = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("ShipmentDetails",
                            "orderdetails/shipment/{shipmentId}",
                            new { controller = "Order", action = "ShipmentDetails" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("ReturnRequest",
                            "returnrequest/{orderId}",
                            new { controller = "ReturnRequest", action = "ReturnRequest" },
                            new { orderId = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("ReOrder",
                            "reorder/{orderId}",
                            new { controller = "Order", action = "ReOrder" },
                            new { orderId = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("GetOrderPdfInvoice",
                            "orderdetails/pdf/{orderId}",
                            new { controller = "Order", action = "GetPdfInvoice" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("PrintOrderDetails",
                            "orderdetails/print/{orderId}",
                            new { controller = "Order", action = "PrintOrderDetails" },
                            new[] { "Seagull.Web.Controllers" });
            //order downloads
            routes.MapRoute("GetDownload",
                            "download/getdownload/{orderItemId}/{agree}",
                            new { controller = "Download", action = "GetDownload", agree = UrlParameter.Optional },
                            new { orderItemId = new GuidConstraint(false) },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapRoute("GetLicense",
                            "download/getlicense/{orderItemId}/",
                            new { controller = "Download", action = "GetLicense" },
                            new { orderItemId = new GuidConstraint(false) },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("DownloadUserAgreement",
                            "user/useragreement/{orderItemId}",
                            new { controller = "User", action = "UserAgreement" },
                            new { orderItemId = new GuidConstraint(false) },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapRoute("GetOrderNoteFile",
                            "download/ordernotefile/{ordernoteid}",
                            new { controller = "Download", action = "GetOrderNoteFile" },
                            new { ordernoteid = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });

            //contact vendor
            routes.MapLocalizedRoute("ContactVendor",
                            "contactvendor/{vendorId}",
                            new { controller = "Common", action = "ContactVendor" },
                            new[] { "Seagull.Web.Controllers" });
            //apply for vendor account
            routes.MapLocalizedRoute("ApplyVendorAccount",
                            "vendor/apply",
                            new { controller = "Vendor", action = "ApplyVendor" },
                            new[] { "Seagull.Web.Controllers" });
            //vendor info
            routes.MapLocalizedRoute("UserVendorInfo",
                            "user/vendorinfo",
                            new { controller = "Vendor", action = "Info" },
                            new[] { "Seagull.Web.Controllers" });

            //poll vote AJAX link
            routes.MapLocalizedRoute("PollVote",
                            "poll/vote",
                            new { controller = "Poll", action = "Vote" },
                            new[] { "Seagull.Web.Controllers" });

            //comparing products
            routes.MapLocalizedRoute("RemoveProductFromCompareList",
                            "compareproducts/remove/{productId}",
                            new { controller = "Product", action = "RemoveProductFromCompareList" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("ClearCompareList",
                            "clearcomparelist/",
                            new { controller = "Product", action = "ClearCompareList" },
                            new[] { "Seagull.Web.Controllers" });

            //new RSS
            routes.MapLocalizedRoute("NewProductsRSS",
                            "newproducts/rss",
                            new { controller = "Product", action = "NewProductsRss" },
                            new[] { "Seagull.Web.Controllers" });
            
            //get state list by country ID  (AJAX link)
            routes.MapRoute("GetStatesByCountryId",
                            "country/getstatesbycountryid/",
                            new { controller = "Country", action = "GetStatesByCountryId" },
                            new[] { "Seagull.Web.Controllers" });

            //EU Cookie law accept button handler (AJAX link)
            routes.MapRoute("EuCookieLawAccept",
                            "eucookielawaccept",
                            new { controller = "Common", action = "EuCookieLawAccept" },
                            new[] { "Seagull.Web.Controllers" });


            //product attributes with "upload file" type
            routes.MapLocalizedRoute("UploadFileProductAttribute",
                            "uploadfileproductattribute/{attributeId}",
                            new { controller = "ShoppingCart", action = "UploadFileProductAttribute" },
                            new { attributeId = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            //checkout attributes with "upload file" type
            routes.MapLocalizedRoute("UploadFileCheckoutAttribute",
                            "uploadfilecheckoutattribute/{attributeId}",
                            new { controller = "ShoppingCart", action = "UploadFileCheckoutAttribute" },
                            new { attributeId = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            //return request with "upload file" tsupport
            routes.MapLocalizedRoute("UploadFileReturnRequest",
                            "uploadfilereturnrequest",
                            new { controller = "ReturnRequest", action = "UploadFileReturnRequest" },
                            new[] { "Seagull.Web.Controllers" });

            //forums
            routes.MapLocalizedRoute("ActiveDiscussions",
                            "boards/activediscussions",
                            new { controller = "Boards", action = "ActiveDiscussions" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("ActiveDiscussionsPaged",
                            "boards/activediscussions/page/{page}",
                            new { controller = "Boards", action = "ActiveDiscussions", page = UrlParameter.Optional },
                            new { page = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("ActiveDiscussionsRSS",
                            "boards/activediscussionsrss",
                            new { controller = "Boards", action = "ActiveDiscussionsRSS" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("PostEdit",
                            "boards/postedit/{id}",
                            new { controller = "Boards", action = "PostEdit" },
                            new { id = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("PostDelete",
                            "boards/postdelete/{id}",
                            new { controller = "Boards", action = "PostDelete" },
                            new { id = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("PostCreate",
                            "boards/postcreate/{id}",
                            new { controller = "Boards", action = "PostCreate" },
                            new { id = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("PostCreateQuote",
                            "boards/postcreate/{id}/{quote}",
                            new { controller = "Boards", action = "PostCreate" },
                            new { id = @"\d+", quote = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("TopicEdit",
                            "boards/topicedit/{id}",
                            new { controller = "Boards", action = "TopicEdit" },
                            new { id = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("TopicDelete",
                            "boards/topicdelete/{id}",
                            new { controller = "Boards", action = "TopicDelete" },
                            new { id = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("TopicCreate",
                            "boards/topiccreate/{id}",
                            new { controller = "Boards", action = "TopicCreate" },
                            new { id = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("TopicMove",
                            "boards/topicmove/{id}",
                            new { controller = "Boards", action = "TopicMove" },
                            new { id = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("TopicWatch",
                            "boards/topicwatch/{id}",
                            new { controller = "Boards", action = "TopicWatch" },
                            new { id = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("TopicSlug",
                            "boards/topic/{id}/{slug}",
                            new { controller = "Boards", action = "Topic", slug = UrlParameter.Optional },
                            new { id = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("TopicSlugPaged",
                            "boards/topic/{id}/{slug}/page/{page}",
                            new { controller = "Boards", action = "Topic", slug = UrlParameter.Optional, page = UrlParameter.Optional },
                            new { id = @"\d+", page = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("ForumWatch",
                            "boards/forumwatch/{id}",
                            new { controller = "Boards", action = "ForumWatch" },
                            new { id = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("ForumRSS",
                            "boards/forumrss/{id}",
                            new { controller = "Boards", action = "ForumRSS" },
                            new { id = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("ForumSlug",
                            "boards/forum/{id}/{slug}",
                            new { controller = "Boards", action = "Forum", slug = UrlParameter.Optional },
                            new { id = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("ForumSlugPaged",
                            "boards/forum/{id}/{slug}/page/{page}",
                            new { controller = "Boards", action = "Forum", slug = UrlParameter.Optional, page = UrlParameter.Optional },
                            new { id = @"\d+", page = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("ForumGroupSlug",
                            "boards/forumgroup/{id}/{slug}",
                            new { controller = "Boards", action = "ForumGroup", slug = UrlParameter.Optional },
                            new { id = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("Search",
                            "boards/search",
                            new { controller = "Boards", action = "Search" },
                            new[] { "Seagull.Web.Controllers" });

            //private messages
            routes.MapLocalizedRoute("PrivateMessages",
                            "privatemessages/{tab}",
                            new { controller = "PrivateMessages", action = "Index", tab = UrlParameter.Optional },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("PrivateMessagesPaged",
                            "privatemessages/{tab}/page/{page}",
                            new { controller = "PrivateMessages", action = "Index", tab = UrlParameter.Optional },
                            new { page = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("PrivateMessagesInbox",
                            "inboxupdate",
                            new { controller = "PrivateMessages", action = "InboxUpdate" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("PrivateMessagesSent",
                            "sentupdate",
                            new { controller = "PrivateMessages", action = "SentUpdate" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("SendPM",
                            "sendpm/{toUserId}",
                            new { controller = "PrivateMessages", action = "SendPM" },
                            new { toUserId = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("SendPMReply",
                            "sendpm/{toUserId}/{replyToMessageId}",
                            new { controller = "PrivateMessages", action = "SendPM" },
                            new { toUserId = @"\d+", replyToMessageId = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("ViewPM",
                            "viewpm/{privateMessageId}",
                            new { controller = "PrivateMessages", action = "ViewPM" },
                            new { privateMessageId = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("DeletePM",
                            "deletepm/{privateMessageId}",
                            new { controller = "PrivateMessages", action = "DeletePM" },
                            new { privateMessageId = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });

            //activate newsletters
            routes.MapLocalizedRoute("NewsletterActivation",
                            "newsletter/subscriptionactivation/{token}/{active}",
                            new { controller = "Newsletter", action = "SubscriptionActivation" },
                            new { token = new GuidConstraint(false) },
                            new[] { "Seagull.Web.Controllers" });

            //robots.txt
            routes.MapRoute("robots.txt",
                            "robots.txt",
                            new { controller = "Common", action = "RobotsTextFile" },
                            new[] { "Seagull.Web.Controllers" });

            //sitemap (XML)
            routes.MapLocalizedRoute("sitemap.xml",
                            "sitemap.xml",
                            new { controller = "Common", action = "SitemapXml" },
                            new[] { "Seagull.Web.Controllers" });
            routes.MapLocalizedRoute("sitemap-indexed.xml",
                            "sitemap-{Id}.xml",
                            new { controller = "Common", action = "SitemapXml" },
                            new { Id = @"\d+" },
                            new[] { "Seagull.Web.Controllers" });

            //store closed
            routes.MapLocalizedRoute("StoreClosed",
                            "storeclosed",
                            new { controller = "Common", action = "StoreClosed" },
                            new[] { "Seagull.Web.Controllers" });

            //install
            routes.MapRoute("Installation",
                            "install",
                            new { controller = "Install", action = "Index" },
                            new[] { "Seagull.Web.Controllers" });
            
            //page not found
            routes.MapLocalizedRoute("PageNotFound",
                            "page-not-found",
                            new { controller = "Common", action = "PageNotFound" },
                            new[] { "Seagull.Web.Controllers" });
        }

        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
