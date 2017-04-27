using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;
using Moq;
using MyStore.Domain.Abstract;
using MyStore.Domain.Concrete;
using MyStore.Domain.Entities;


namespace MyStore.WebUI.Infostructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernal;

        public NinjectDependencyResolver(IKernel kernalParam)
        {
            kernal = kernalParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernal.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernal.GetAll(serviceType);
        }

        private void AddBindings()
        {
            kernal.Bind<IProductRepository>().To<EFProductRepository>();
        }
    }
}