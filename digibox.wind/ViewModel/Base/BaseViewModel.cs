using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.wind.ViewModel.Base
{
		public class BaseViewModel : ObjectBase
		{
				protected void ValidateProperty<T>(T value, string name)
				{
						Validator.ValidateProperty(value, new ValidationContext(null, null, null)
						{
								MemberName = name
						}); ;
				}

		}
}
