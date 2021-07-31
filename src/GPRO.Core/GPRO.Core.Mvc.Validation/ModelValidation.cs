using System;
using System.Collections.Generic;
using System.Web.Mvc;
namespace GPRO.Core.Mvc.Validation
{
	public class ModelValidation
	{
		public static IEnumerable<Error> Validate(object objValidate, ControllerContext controllerContext)
		{
			ModelMetadata metadataForType = ModelMetadataProviders.Current.GetMetadataForType(() => objValidate, objValidate.GetType());
			foreach (ModelMetadata current in metadataForType.Properties)
			{
				foreach (ModelValidator current2 in current.GetValidators(controllerContext))
				{
					foreach (ModelValidationResult current3 in current2.Validate(metadataForType.Model))
					{
						yield return new Error
						{
							MemberName = current.PropertyName,
							Message = current3.Message
						};
					}
				}
			}
			yield break;
		}
	}
}
