# UnityCore
Core boilerplate code I use in Unity, including singletons, smart behaviours etc.  Most people come here for the threading bits so keeping this short.

# UnityCore.Threading
Do away with coroutines, and start using Tasks in Unity.
* Use Tasks to manage async code
* Same code for async in **Editor** and in the **Unity Player**
* Async in plain classes 
* Batch async in Parallel
* Weave back and forth between the main thread 
* Return values on async!
* No more
    * Writing one set of code for the editor another for the player
    * Passing around a MonoBehaviour for coroutines
    * Iterators through your code for Coroutines
    * Coroutines - Goodbye!
    * Writing classes just to handle async logic 
    * Callbacks just to get return values

```csharp
public class Tester
{
    public static void Test()
    {
        // First Task runs off the main thread
        Task.Factory.StartNew(() =>
        {
            Debug.Log("This isn't on the main thread");
        }).
        ContinueWith(task =>
        {
            // Still off the main thread for some async
            // i.e. imagine we retrieved some json 
            Debug.Log("Neither is this");
            return "{answer: 42}";
        }).
        ContinueWithOnMainThread(task =>
        {
            // Now jumping onto the main thread 
            // and we have the result from the previous thread!
            Debug.Log(task.Result);

            // And we can do main thread only tasks
            Debug.Log(Application.isEditor);

            // And we can also pass values off the main thread
            return new Vector2(200,200);
        }).
        ContinueWith(task =>
        {
            // And voila, we now have the result from the main thread, 
            // back off and can work async
            Debug.Log(task.Result);
        });
    }
}
```

## UnityCore.Threading Guide
### Creating an Async Task
It's pretty easy to create an async Task, you simply just use `Task.Factory.StartNew(Action)` like below.
```csharp
Task.Factory.StartNew(() =>
{
    Debug.Log("This isn't on the main thread");
});
```
This simply runs the debug statement off the main thread.  This might be more complex, like loading a file, or initialisation.

### Continuing
Often you will want to do something once that async Task is complete.  You can follow up easily with `ContinueWith`.
```csharp
// First Task runs off the main thread
Task.Factory.StartNew(() =>
{
    Debug.Log("This isn't on the main thread");
}).
ContinueWith(task =>
{
    // Still off the main thread for some async
    // i.e. imagine we retrieved some json 
    Debug.Log("Neither is this");
    return "{answer: 42}";
}).
```
`ContinueWith` just runs the next Action or Func<T> off the main thread.  In the example above, it logs to debug, and returns some simple JSON.  This `ContinueWith` could have, for example, performed a web request and retrieved the JSON from a live source, causing a long running thread, perfect for running asycn.
### Passing Data Between Threads
Now that we have a returned result, imagine you need to do something on the main thread.  It may be PNG data that you want to load into a `Texture2D`, very common for player avatar icons.  Let's look at the code below where we use `ContinueWithOnMainThread`.
```csharp
// First Task runs off the main thread
Task.Factory.StartNew(() =>
{
    Debug.Log("This isn't on the main thread");
}).
ContinueWith(task =>
{
    // Still off the main thread for some async
    // i.e. imagine we retrieved some json 
    Debug.Log("Neither is this");
    return "{answer: 42}";
}).
ContinueWithOnMainThread(task =>
{
    // Now jumping onto the main thread 
    // and we have the result from the previous thread!
    Debug.Log(task.Result);

    // And we can do main thread only tasks
    Debug.Log(Application.isEditor);

    // And we can also pass values off the main thread
    return new Vector2(200, 200);
}).
```
Here we can get access to the JSON returned in the previous task via task.Result.  And to prove we are on the main thread, the cdoe aboce does a call to `Application.isEditor` which throws and exception if not on the main thread.
### Passing Data Back From The Main Thread
And now, let's look at the flow back off the main thread

```csharp
// First Task runs off the main thread
Task.Factory.StartNew(() =>
{
    Debug.Log("This isn't on the main thread");
}).
ContinueWith(task =>
{
    // Still off the main thread for some async
    // i.e. imagine we retrieved some json 
    Debug.Log("Neither is this");
    return "{answer: 42}";
}).
ContinueWithOnMainThread(task =>
{
    // Now jumping onto the main thread 
    // and we have the result from the previous thread!
    Debug.Log(task.Result);

    // And we can do main thread only tasks
    Debug.Log(Application.isEditor);

    // And we can also pass values off the main thread
    return new Vector2(200,200);
}).
ContinueWith(task =>
{
    // And voila, we now have the result from the main thread, 
    // back off and can work async
    Debug.Log(task.Result);
});
```
Here you can see that we just go `ContinueWith` and voila, you're back off the main thread, with the data obtained in the main thread.  This could be for example, the dimensions of the `Texture2D` downloaded, as `Texture2D.width` can only be called on the main thread.

## Why Not Use TaskScheduler.FromCurrentSynchronizationContext() ?
There are a few issues with this, including implied contexts, and the varying Engine loop.
### Implied Context
Let's say you are capturing device input from an external source, or possibly listening for a message bus, when this event fires, you may want to do something on the main thread.  In this case `TaskScheduler.FromCurrentSynchronizationContext` is not on the main game thread, so it simply won't work.
### The Engine Loop
Implied context aside, even if you just gave it the main game thread to run the `Task` on, the `Task` itself needs to run inside the `Update()` phase on the game loop.  While some calls may work when throwing back to the thread, other things will not.  In addition, you can profile these calls in the profiler if you put them in the update loop, ensuring your threading is predictable, and knowing the overhead per frame.
### How Can I Run The Callback on FixedUpdate()? 
Good point.  Add it to the list for now.  For now you could trigger this from your Update call, however that is just more thread / state weaving which this libary is meant to remove from your coding. 

## Multiple Tasks
Let's say you create a few tasks, you can wait for them all to complete by using `Task.Factory.ContinueWhenAll`.
```csharp
var t1 = Task.Factory.StartNew( () => ... );
var t2 = Task.Factory.StartNew( () => ... );
var tasks = new Task[]{t1, t2};
Task.Factory.ContinueWhenAll(tasks, allTasks =>
{
    foreach(var task in allTasks)
    {
        //... do something
    }
});
```
This is the base functionality out of the Task library. 

**NOTE** - There is no `ContinueWhenAllOnMainThread` implementation yet.

## Parallel
Parallel is very straight forward, and it just uses the base Task Parallel Library 3.5 implementation.  Most of the guidance can be found there, however to give you an idea of what you can do, an example is provided below.

```csharp
Parallel.For(0, 100, index =>
{
    Debug.Log(index);
});
```
The call to `Parallel.For(int low, int high, Action)` will run the Debug.Log(index) 101 times.  This can be handy when downloading a collection of online resources.  For me it's usually when prefetching detailed information for a set of results.