# laget.Limiter
A generic rate limiter, Useful for API clients, web crawling, or other tasks that need to be throttled...

![Nuget](https://img.shields.io/nuget/v/laget.Limiter)
![Nuget](https://img.shields.io/nuget/dt/laget.Limiter)

## Configuration
> This example is shown using Autofac, since this is the go to IoC for us.
```c#
builder.Register<IAuthorizationLimit>(c =>
    new AuthorizationLimit(new MemoryStore(),
        new StandardLimit(300, TimeSpan.FromHours(3)))
).SingleInstance();
```

## Usage
> You can specifiy that a limiter should be used by the following
```c#
_limiter.Limit(() =>
{
    ...
});
```

> You can also specify that a kown amount of call will be hit by the following
```c#
_limiter.Limit(2, () =>
{
    ...
});
```

> You can aslo specify additional calls by the following
```c#
limiter.Limit(() =>
{
    limiter.Register(3);
});
```

> You can aslo use it asynchronous by the following
```c#
await limiter.LimitAsync(() => Task.CompletedTask);
```
