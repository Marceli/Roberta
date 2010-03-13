using System.Collections.Generic;

namespace Bll
{
    public interface IValidable
	{
		bool Validate(List<string> ValidationError);
	}
}
