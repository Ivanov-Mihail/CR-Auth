using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
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
		public Auth()
		{

		}
		public Auth(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public List<PayloadRows> PayloadRows = new List<PayloadRows>();

		public async Task<AuthUser> GetPayloadAfterValidation(string bearerToken)
		{
			var token = bearerToken.Replace("Bearer ", "");
			var jsonWebToken = new JsonWebToken(token);

			// for test
			var jsonWebToken1 = new JsonWebToken("eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6IjNkOWVkNDI0YTkxOTlmYWJlMzkyNDc2NjQ4ZjU4MTk1NjFkOWQ2NGExYTNkOTA4MDk2M2IxZTgyY2JkNTQzOTIzMTEwYjFmOTcwY2FhZTQwIn0.eyJhdWQiOiIxIiwianRpIjoiM2Q5ZWQ0MjRhOTE5OWZhYmUzOTI0NzY2NDhmNTgxOTU2MWQ5ZDY0YTFhM2Q5MDgwOTYzYjFlODJjYmQ1NDM5MjMxMTBiMWY5NzBjYWFlNDAiLCJpYXQiOjE1OTE3MDI3MzYsIm5iZiI6MTU5MTcwMjczNiwiZXhwIjoxNjIzMjM4NzM2LCJzdWIiOiIyMSIsInNjb3BlcyI6W10sImlkIjoyMSwicm9sZXMiOlsic3VwZXItYWRtaW4iLCJhZG1pbiIsImNsaWVudCJdLCJwZXJtaXNzaW9ucyI6W119.LG8oMjkW_zi1vvcnltGWjSbLFJnST3auESyVdmSHezHNKa93sKKvOTkiVscC5QQSDNyBds-jkQwyUdfkyQkQctU3dXFZV_Ji2nYa42voWsDl-JLabsU-G9Gd9PzDr96Z1gN6zaOZiSDpekdDtn9z_KvFcISjfr6SAzfPkvCStpc0-YrAys1O2HfNsJqomgDIrzbTuAjVfJ7bThzdVE-ilNtK6jFbmoNKsZAHnhLKxHR4txnI-SOmOjgsmzhjblYg2Esm1c1c_z0D1LyisnVGIz15s1kPprI329tjlfOIrYm4PZYnMeXAeHSxVgjrIsCd5VCLLxyvYs6Hfcbp9VFCc-2z9KHpiKnlq0tDruEfqMp5Bp7lzMG_XXMKVotL0pryYISbYqQUVX-65X8rdTdM7qo_LUPYJQfwOhr6951bMEeCrVyfvNvizV26g2KRHghiziFH0gnANF8lrUIOFZMzG3BhOEPw08gEv6ZgdmtqBa68YbhB37abEpEEdUgbTUa7_CLhLs4aAg20h3t563n9f0dlUpEgtwgKxtMB7VZ-suBeV819Eo814he9D3iEr-Z99kt49vdiZUUIikwv12VLyPhjzyziTbHUPdtGPIpxYrH9HfL4upF2M9SqId9m_NsWanXK6zRSlXcYt3n4oFeBB-Xn5ggfY3nqjiBFP37xXUk");

			// 1st implementation
			AuthUser authUserData;
			{
				string id = null;
				string[] roles = null;
				string[] permissions = null;

				jsonWebToken.TryGetPayloadValue<string>("id", out id);
				jsonWebToken.TryGetPayloadValue<string[]>("roles", out roles);
				jsonWebToken.TryGetPayloadValue<string[]>("permissions", out permissions);
				authUserData = new AuthUser(id, roles, permissions);

			}


			var securityTokenHandler = new JwtSecurityTokenHandler();
			var publicKey = _configuration["Jwt:Key"];
			var options = GetTokenValidationParameters(publicKey);

			var claimPrincipal = await Task.Run(() =>
				securityTokenHandler.ValidateToken(bearerToken, options, out var securityToken));


			if (null != claimPrincipal)
			{
				foreach (var claim in claimPrincipal.Claims)
				{
					var row = new PayloadRows
					{
						Key = claim.Type,
						Value = claim.Value
					};
					PayloadRows.Add(row);
				}
			}

			return authUserData;

		}

		private readonly IConfiguration _configuration;
		private TokenValidationParameters GetTokenValidationParameters(string key)
		{

			var rs256Token = key.Replace("-----\nBEGIN PUBLIC KEY-----", "");
			rs256Token = rs256Token.Replace("\n-----END PUBLIC KEY-----", "");

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
