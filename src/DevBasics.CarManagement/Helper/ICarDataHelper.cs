using DevBasics.CarManagement.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBasics.CarManagement.Helper
{
	public interface ICarDataHelper
	{
		bool HasMissingData(CarRegistrationModel car);
	}
}