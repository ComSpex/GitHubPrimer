#region Copyright Syncfusion Inc. 2001-2016.
// Copyright Syncfusion Inc. 2001-2016. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SyncfusionASPNETWebApplication1.Models;

namespace SyncfusionASPNETWebApplication1.Account {
	public partial class TwoFactorAuthenticationSignIn:System.Web.UI.Page {
		private ApplicationSignInManager signinManager;
		private ApplicationUserManager manager;

		public TwoFactorAuthenticationSignIn() {
			manager=Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
			signinManager=Context.GetOwinContext().GetUserManager<ApplicationSignInManager>();
		}

		protected void Page_Load(object sender,EventArgs e) {
			var userId = signinManager.GetVerifiedUserId<ApplicationUser,string>();
			if(userId==null) {
				Response.Redirect("/Account/Error",true);
			}
			var userFactors = manager.GetValidTwoFactorProviders(userId);
			Providers.DataSource=userFactors.Select(x => x).ToList();
			Providers.DataBind();
		}

		protected void CodeSubmit_Click(object sender,EventArgs e) {
			bool rememberMe = false;
			bool.TryParse(Request.QueryString["RememberMe"],out rememberMe);

			var result = signinManager.TwoFactorSignIn<ApplicationUser,string>(SelectedProvider.Value,Code.Text,isPersistent: rememberMe,rememberBrowser: RememberBrowser.Checked);
			switch(result) {
				case SignInStatus.Success:
					IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"],Response);
					break;
				case SignInStatus.LockedOut:
					Response.Redirect("/Account/Lockout");
					break;
				case SignInStatus.Failure:
				default:
					FailureText.Text="Invalid code";
					ErrorMessage.Visible=true;
					break;
			}
		}

		protected void ProviderSubmit_Click(object sender,EventArgs e) {
			if(!signinManager.SendTwoFactorCode(Providers.SelectedValue)) {
				Response.Redirect("/Account/Error");
			}

			var user = manager.FindById(signinManager.GetVerifiedUserId<ApplicationUser,string>());
			if(user!=null) {
				var code = manager.GenerateTwoFactorToken(user.Id,Providers.SelectedValue);
			}

			SelectedProvider.Value=Providers.SelectedValue;
			sendcode.Visible=false;
			verifycode.Visible=true;
		}
	}
}