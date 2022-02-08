using LNF;
using LNF.CommonTools;
using LNF.Data;
using LNF.Web;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

namespace sselIndReports
{
    public partial class _500 : Page
    {
        public IProvider Provider => ServiceProvider.Current;

        protected void Page_Load(object sender, EventArgs e)
        {
            ContextBase = new HttpContextWrapper(Context);

            var lastEx = Server.GetLastError();

            if (lastEx != null)
            {
                var innerEx = lastEx.InnerException;
                if (innerEx != null)
                    HandleError(innerEx);
                else
                    HandleError(lastEx);
            }
            else
            {
                errors.Add(new ErrorItem { Message = "No error found.", StackTrace = string.Empty });
            }

            rptErrors.DataSource = errors;
            rptErrors.DataBind();
        }

        public HttpContextBase ContextBase { get; private set; }

        private IList<ErrorItem> errors = new List<ErrorItem>();

        private void HandleError(Exception ex)
        {
            AddError(ex);
            var currentUser = GetCurrentUser();
            SendErrorEmail(ex, currentUser);
        }

        private void AddError(Exception ex)
        {
            errors.Add(new ErrorItem { Message = ex.Message, StackTrace = ex.StackTrace });
        }

        private IClient GetCurrentUser()
        {
            IClient currentUser = null;

            try
            {
                currentUser = ContextBase.CurrentUser(Provider);
            }
            catch (Exception ex)
            {
                AddError(ex);
            }

            return currentUser;
        }

        private void SendErrorEmail(Exception err, IClient currentUser)
        {
            try
            {
                var app = ServiceProvider.Current.Log.Name;
                SendEmail.SendErrorEmail(err, string.Empty, currentUser, app, ContextBase.CurrentIP(), ContextBase.Request.Url);
            }
            catch (Exception ex)
            {
                AddError(ex);
            }
        }
    }

    public class ErrorItem
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}