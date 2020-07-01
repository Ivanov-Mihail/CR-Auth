using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace CR_Auth
{
	public class Auth
	{
		private AuthUser _authUserData;
		public string PublicKey { get; set; }
		public Auth(string publicKey)
		{
			PublicKey = publicKey;
		}


		public AuthUser GetPayloadWithoutTokenValidation(string bearerToken)
		{
			var token = bearerToken.Replace("Bearer ", "");
			var jsonWebToken = new JsonWebToken(token);

			jsonWebToken.TryGetPayloadValue<string>("id", out string id);
			jsonWebToken.TryGetPayloadValue<string[]>("roles", out var roles);
			jsonWebToken.TryGetPayloadValue<string[]>("permissions", out var permissions);
			var authUserData = new AuthUser(id, roles, permissions);

			return authUserData;
		}

		public AuthUser GetPayloadFromValidatedToken(string bearerToken)
		{
			var options = GetTokenValidationParameters(PublicKey);
			var token = bearerToken.Replace("Bearer ", "");
			var securityTokenHandler = new JwtSecurityTokenHandler();
			var claimPrincipal = securityTokenHandler.ValidateToken(token, options, out var securityToken);
			//
			if (claimPrincipal == null) return _authUserData;
			_authUserData = new AuthUser();
			//
			var userId = claimPrincipal.Claims.First(claim => claim.Type == "id").Value;
			_authUserData.UserId = userId;
			//
			var roles = claimPrincipal.Claims.Where(claim => claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").ToArray();
			var rolesLength = roles.Length;
			_authUserData.Roles = new string[rolesLength];
			for (var a = 0; a < rolesLength; a++)
			{
				_authUserData.Roles[a] = roles[a].Value;
			}
			//
			var permissions = claimPrincipal.Claims.Where(claim => claim.Type == "permissions").ToArray();
			var permissionsLength = permissions.Length;
			_authUserData.Permissions = new string[permissionsLength];
			for (var b = 0; b < permissionsLength; b++)
			{
				_authUserData.Permissions[b] = permissions[b].Value;
			}
			//
			return _authUserData;
		}

		private TokenValidationParameters GetTokenValidationParameters(string key)
		{

			var rs256Token = key.Replace("-----BEGIN PUBLIC KEY-----", "");
			rs256Token = rs256Token.Replace("-----END PUBLIC KEY-----", "");

			var keyBytes = Convert.FromBase64String(rs256Token);
			var asymmetricKeyParameter = PublicKeyFactory.CreateKey(keyBytes);
			var rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;
			var rsaParameters = new RSAParameters
			{
				Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned(),
				Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned()
			};
			var rsa = new RSACryptoServiceProvider();

			rsa.ImportParameters(rsaParameters);

			var validationParameters = new TokenValidationParameters()
			{
				RequireExpirationTime = false,
				RequireSignedTokens = true,
				ValidateAudience = false,
				ValidateIssuer = false,
				IssuerSigningKey = new RsaSecurityKey(rsa),
			};

			return validationParameters;
		}
	}
}
