using System.Data.Entity.ModelConfiguration;

namespace Seagull.Data.Mapping
{
    public abstract class SeagullEntityTypeConfiguration<T> : EntityTypeConfiguration<T> where T : class
    {
        protected SeagullEntityTypeConfiguration()
        {
            PostInitialize();
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {
            
        }
    }
}