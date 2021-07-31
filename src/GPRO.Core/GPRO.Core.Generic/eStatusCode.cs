using System;
namespace GPRO.Core.Generic
{
	public enum eStatusCode
	{
		OK = 200,
		BadRequest = 400,
		Unauthorized,
		Forbidden = 403,
		NotFound,
		InternalServerError = 500
	}
}
