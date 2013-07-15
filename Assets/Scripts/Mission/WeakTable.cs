using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Threading;
 
/// <summary>
/// Weak entry for the table
/// </summary>
class WeakEntry 
{
    /// <summary>
    /// The weak reference to the collectable object
    /// </summary>
    public WeakReference weakReference;
    /// <summary>
    /// The associated object with the reference
    /// </summary>
    public object associate;
}
 
/// <summary>
/// A weak table for a particular type of associated class
/// </summary>
public class WeakTable<T> : IDisposable where T : class, new()
{
 
    //When finished we can dispose of the notification
    public void Dispose()
    {
        GCNotification.GCDone -= Collected;
    }
 
    /// <summary>
    /// The references held by the table
    /// </summary>
    Dictionary<int, List<WeakEntry>> references = new Dictionary<int, List<WeakEntry>>();
 
    /// <summary>
    /// Gets an associated object give an index of another object
    /// </summary>
    /// <param name='index'>
    /// The object to use as an index
    /// </param>
    public T this[object index]
    {
        get
        {
            //Get the hash code of the indexed object
            var hash = index.GetHashCode();
            List<WeakEntry> entries;
            //Try to get a reference to it
            if(!references.TryGetValue(hash, out entries))
            {
                //If we failed then create a new entry
                references[hash] = entries = new List<WeakEntry>();
            }
            //Try to get an associated object of the right type for this
            //indexer/make sure it is still alive
            var item = entries.FirstOrDefault(e=>e.weakReference.IsAlive && e.weakReference.Target == index);
            //Check if we got one
            if(item == null) 
            {
                //If we didn't then create a new one
                entries.Add(item = new WeakEntry { weakReference = new WeakReference(index), associate = new T() });
            }
            //Return the associated object
            return (T)item.associate;
        }
 
    }
 
    /// <summary>
    /// Get an associate given an indexing object
    /// </summary>
    /// <param name='index'>
    /// The object to find the associate for
    /// </param>
    /// <typeparam name='T2'>
    /// The type of associate to find
    /// </typeparam>
    public T2 Get<T2>(object index) where T2 : T, new()
    {
        //Get the hash code of the indexing object
        var hash = index.GetHashCode();
        List<WeakEntry> entries;
        //See if we have a reference already
        if(!references.TryGetValue(hash, out entries))
        {   
            //If not the create the reference list
            references[hash] = entries = new List<WeakEntry>();
        }
        //See if we have an object of the correct type and that the
        //reference is still alive
        var item = entries.FirstOrDefault(e=>e.weakReference.IsAlive && e.weakReference.Target == index && e.associate is T2);
        if(item == null) 
        {
            //If not create one
            entries.Add(item = new WeakEntry { weakReference = new WeakReference(index), associate = new T2() });
        }
        //Return the associate
        return (T2)item.associate;
    }
 
    public WeakTable() 
    {
        //Setup garbage collection notification
        GCNotification.GCDone += Collected;
    }
 
    /// <summary>
    /// Called when the garbage has been collected
    /// </summary>
    /// <param name='generation'>
    /// The generation that was collected
    /// </param>
    void Collected(int generation)
    {
        //Remove the references which are no longer alive
 
        //Scan each reference list
        foreach(var p in references)
        {
            //Scan each item in the references and remove
            //items that are missing
            removeEntries.Clear();
            foreach(var r in p.Value.Where(r=>!r.weakReference.IsAlive))
                removeEntries.Add(r);
            foreach(var entry in removeEntries)
            {
                if(entry.associate is IDisposable)
                    (entry.associate as IDisposable).Dispose();                      
                p.Value.Remove(entry);
            }
        }
    }
 
    List<WeakEntry> removeEntries = new List<WeakEntry>();
 
}
 
/// <summary>
/// Extension class to support getting weak tables easily
/// </summary>
public static class Extension
{
    static Dictionary<Type, WeakTable<object>> extensions = new Dictionary<Type, WeakTable<object>>();
 
    /// <summary>
    /// Get an associate for a particular object
    /// </summary>
    /// <param name='reference'>
    /// The object whose associate should be found
    /// </param>
    /// <param name='create'>
    /// Whether the associate should be created (defaults true)
    /// </param>
    /// <typeparam name='T'>
    /// The type of associate
    /// </typeparam>
    public static T Get<T>(this object reference, bool create = true) where T : class, new()
    {
        WeakTable<object> references;
        //Try to get a weaktable for the reference object
        if(!extensions.TryGetValue(reference.GetType(), out references))
        {
            //Verify that we should be creating it if missing
            if(!create)
                return null;
            //Create a new table
            extensions[reference.GetType()] = references = new WeakTable<object>();
        }
        //Get the associate from the table
        return (T)references.Get<T>(reference);
    }
 
}
 
//The following class is from:
//Jeff Richter - http://www.wintellect.com/CS/blogs/jeffreyr/archive/2009/12/22/receiving-notifications-garbage-collections-occur.aspx
public static class GCNotification {
   private static Action<Int32> s_gcDone = null; // The event’s field
   public static event Action<Int32> GCDone {
      add {
         // If there were no registered delegates before, start reporting notifications now 
         if (s_gcDone == null) { new GenObject(0); new GenObject(2); }
         s_gcDone += value;
      }
      remove { s_gcDone -= value; }
   }
   private sealed class GenObject {
      private Int32 m_generation;
      public GenObject(Int32 generation) { m_generation = generation; }
      ~GenObject() { // This is the Finalize method
         // If this object is in the generation we want (or higher), 
         // notify the delegates that a GC just completed
         if (GC.GetGeneration(this) >= m_generation) {
            //Thread safe get of the s_gcDone delegate - will not be interrupted
            Action<Int32> temp = Interlocked.CompareExchange(ref s_gcDone, null, null);
            //Fire the event
            if (temp != null) temp(m_generation);
         }
         // Keep reporting notifications if there is at least one delegate
         // registered, the AppDomain isn't unloading, and the process 
         // isn’t shutting down
         if ((s_gcDone != null) && 
            !AppDomain.CurrentDomain.IsFinalizingForUnload() && 
            !Environment.HasShutdownStarted) {
            // For Gen 0, create a new object; for Gen 2, resurrect the
            // object & let the GC call Finalize again the next time Gen 2 is GC'd
            if (m_generation == 0) new GenObject(0);
            else GC.ReRegisterForFinalize(this);
         } else { /* Let the objects go away */ }
      }
   }
}