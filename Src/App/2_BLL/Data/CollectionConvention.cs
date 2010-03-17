using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Core.Data
{
	public class CollectionConvention : ICollectionConvention
	{
		public void Apply(ICollectionInstance instance)
		{
			instance.Cascade.SaveUpdate();
			instance.Inverse();
		}

       
	}
}