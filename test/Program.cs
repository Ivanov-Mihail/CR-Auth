using System;
using CRAP;

namespace test
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{

				Auth ayAuth = new Auth(null);
				ayAuth.GetPayloadAfterValidation("Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6IjlmZjA4Y2EzMzgwZjI0NzNhYzU1MTM3NGY5ZmRjMmUyNGFmZjhmYzY1ZjlhYWYzOWIwY2ZkNDIxMzMzNjIxMTdhM2FmMDA2YTFmZWNlMmQwIn0.eyJhdWQiOiIxIiwianRpIjoiOWZmMDhjYTMzODBmMjQ3M2FjNTUxMzc0ZjlmZGMyZTI0YWZmOGZjNjVmOWFhZjM5YjBjZmQ0MjEzMzM2MjExN2EzYWYwMDZhMWZlY2UyZDAiLCJpYXQiOjE1OTIyOTgzNjEsIm5iZiI6MTU5MjI5ODM2MSwiZXhwIjoxNjIzODM0MzYxLCJzdWIiOiIxIiwic2NvcGVzIjpbXSwiaWQiOjEsInJvbGVzIjpbImFkbWluIiwiY2xpZW50IiwiZHJpdmVyIiwiY29tcGFueSBvd25lciJdLCJwZXJtaXNzaW9ucyI6WyJ2aWV3X3VzZXJzIiwiZWRpdF91c2VycyIsImFkZF91c2VycyIsInZpZXdfY29tcGFuaWVzIiwiZWRpdF9jb21wYW5pZXMiXX0.nH6_Gf8fJza9bxi0xe_o94sCFTaXqJ4jCHQ_kOPn5hiRuMbb5j4CFU8Rg3PB7JKOsp2xj4Vm2CfyNPPuMowhv7DwQyFJ1zABV1s_Wyfu9S4HEOBUGJk7isuEBsk7mY5nSoOGXixgXVABDsm1taLdxpra8gVsltqV2BKUsqCvJdd7bAOV9cF0wWAloJF87k8SrY27VVxZ-6gLW060_kaTl0kc2PmL4v0UYTLQz5YL1mtYznCy6yPiIR1ciio5o7b80KYS1NXLnuNXdfN0kb94uq87hm714m_YObncVQk9kqWL5dwLBFSlVJbmshkILU4wlYGAiiNH7ea_09idO0_Xe_eaIQ8qj6ZwxV83LpINK2vXRFsUS1DFPGbC1ltKDL2OTFVupv6npPO5yn6KCW6_4amdV5pAFqwX1yZqybcRm_LnimNlsfN0id13nXVYu2DzvJ6Vmp-80Xps050wRF1E9INOFRBdRyME_bjriLSezZXPp54PrzaqV_KWJRPTZ-5qvsXrk-7wp8GwW_ZvZZ_vFNLf6pkBD2v5mFgKHXrb0aBmbUkwKo4IqFbi_LrpbgUOm9U1COIx7kjMVg0Sml9zqkwt09YBFI9zUP-2px7LIboXRPzCCZoekwos52xHlYdrZCM7ciIO4em4HUbIAgUE4v5QJlyt4OuQc0RXLUqQ0vQ");

					Console.WriteLine("-- --!");


				AuthCityRideModule arm = new AuthCityRideModule();

				arm.Authenticate();
				arm.JwtTokVerif();

				Console.WriteLine("Hello World!");
				Console.ReadLine();

			}catch
			{
				throw new Exception();
			}
		}
	}
}
