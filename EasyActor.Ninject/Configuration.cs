﻿using Ninject;
using Ninject.Syntax;

namespace EasyActor.Ninject
{
    public static class Configuration
    {
        public static IKernel GetKernel()
        {
            var kernel = new StandardKernel(new NinjectSettings { UseReflectionBasedInjection = true });
            RegisterDependency(kernel);
            return kernel;
        }

        private static void RegisterDependency(IKernel standardKernel)
        {
            var factoryBuilder = new FactoryBuilder();
            var actorFactory = factoryBuilder.GetFactory();
            BindAsActor<IActor, Actor>(standardKernel, actorFactory).InSingletonScope();
        }

        private static IBindingWhenInNamedWithOrOnSyntax<TInterface> BindAsActor<TInterface, T>(IKernel standardKernel, IActorFactory factory) 
            where T: TInterface
            where TInterface: class
        {
            return standardKernel.Bind<TInterface>().ToMethod(ctx => factory.Build<TInterface>(ctx.Kernel.Get<T>()));
        }
    }
}
