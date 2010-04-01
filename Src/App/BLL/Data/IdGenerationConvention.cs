using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Core.Data
{
	public class IdGenerationConvention : IIdConvention
	{
		public void Apply(IIdentityInstance instance)
		{
			instance.GeneratedBy.HiLo("1000");
		}
	}
}