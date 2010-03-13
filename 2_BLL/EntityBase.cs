using System;
using Wilson.ORMapper;

namespace Bll
{
    public abstract class EntityBase
    {
        #region ORMapper Default Methods

        ///<summary>Initializes a new instance of this class</summary>
        public EntityBase()
        {
            Dm.ObjectSpace.StartTracking(this, InitialState.Inserted);
        }

        ///<summary>Save this instance to the persistence store without children with transaction It transaction==null then save without transaction</summary>
        ///<param name="transaction">An instance of a Wilson.ORMapper.Transaction to perform operation with.</param>	
        public virtual void Save(Transaction transacion)
        {
            Save(transacion,false);
        }

        /// <summary>
        /// Save this instance to the persistence store with transaction
        /// </summary>
        /// <param name="t">
        /// An instance of a Wilson.ORMapper.Transaction to perform operation with.
        /// It`s could be null.
        /// </param>
        /// <param name="includeChildren">Include changes to related child instances</param>
        protected virtual void Save(Transaction t, bool includeChildren)
        {
            if (Dm.ObjectSpace.GetObjectState(this) == ObjectState.Unknown)
                Dm.ObjectSpace.StartTracking(this, InitialState.Inserted);

            PersistDepth depth = includeChildren ? PersistDepth.ObjectGraph : PersistDepth.SingleObject;
            if (t != null)
                t.PersistChanges(this, depth);
            else
                Dm.ObjectSpace.PersistChanges(this,depth);
            
                
        }

        
        ///<summary>Delete this instance from the persistence store using a transaction</summary>
        ///<param name="transaction">An instance of a Wilson.ORMapper.Transaction to perform operation with.</param>
        public virtual void Delete(Transaction transaction)
        {
            if (Dm.ObjectSpace.GetObjectState(this) == ObjectState.Unknown)
                return;

            Dm.ObjectSpace.MarkForDeletion(this);
            if(transaction==null)
            {
                Dm.ObjectSpace.PersistChanges(this);
            }else
            {
                transaction.PersistChanges(this);
            }
        }

/*
        /// <summary>Refresh the data for this instance from the persistence store</summary>
        /// <returns>Returns a new instance with the refreshed data or null if instance not tracked</returns>
        /// <example>Resync an instance code fragment
        /// <code>
        /// T instance;
        /// // Some retrieval and update logic
        /// instance = instance.Resync();
        /// </code>
        /// </example>
        public T Resync<T>()
        {
            if (DataManager.ObjectSpace.GetObjectState(this) == ObjectState.Unknown)
                return default(T);

            return (T) DataManager.ObjectSpace.Resync(this);
        }
 */
        public object Resync()
        {
            if (Dm.ObjectSpace.GetObjectState(this) == ObjectState.Unknown)
                return null;

            return Dm.ObjectSpace.Resync(this);
        }

        #endregion
/*
        public T This
        {
            get { return (T) this; }
        }
*/
    }
}