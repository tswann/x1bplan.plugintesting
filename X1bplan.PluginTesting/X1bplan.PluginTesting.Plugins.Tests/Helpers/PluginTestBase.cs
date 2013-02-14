using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Moq;

namespace X1bplan.PluginTesting.Plugins.Tests.Helpers
{
    public abstract class PluginTestBase
    {
        #region Constants
        protected const int STAGE_PREVALIDATION = 10;
        protected const int STAGE_PREOPERATION = 20;
        protected const int STAGE_POSTOPERATION = 40;
        #endregion

        #region Properties
        protected Mock<IServiceProvider> ServiceProviderMock { get; set; }
        protected Mock<IOrganizationService> OrganizationServiceMock { get; set; }
        protected Mock<IOrganizationServiceFactory> OrgServiceFactoryMock { get; set; }
        protected Mock<IPluginExecutionContext> PipelineContextMock { get; set; }
        #endregion

        public PluginTestBase()
        {
            ServiceProviderMock = new Mock<IServiceProvider>();
            OrgServiceFactoryMock = new Mock<IOrganizationServiceFactory>();
            OrganizationServiceMock = new Mock<IOrganizationService>(MockBehavior.Strict);
            PipelineContextMock = new Mock<IPluginExecutionContext>(MockBehavior.Strict);

            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            ServiceProviderMock.Setup(s => s.GetService(typeof(IPluginExecutionContext))).Returns(PipelineContextMock.Object);
            ServiceProviderMock.Setup(s => s.GetService(typeof(IOrganizationServiceFactory))).Returns(OrgServiceFactoryMock.Object);
            OrgServiceFactoryMock.Setup(f => f.CreateOrganizationService(It.IsAny<Guid>())).Returns(OrganizationServiceMock.Object);
        }
    }
}
