using System;
using System.Collections.Generic;
using System.Text;

namespace CR_Auth
{
	public class AuthUser
	{
		public string UserId { get; set; }
		public string[] Roles { get; set; }
		public string[] Permissions { get; set; }

		public AuthUser()
		{

		}

		public AuthUser(string userId, string[] roles, string[] permissions)
		{
			UserId = userId;
			Roles = roles;
			Permissions = permissions;
		}

	}
}
