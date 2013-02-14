using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using NUnit.Framework;
using X1bplan.PluginTesting.X1bplan.PluginTesting.Plugins;

namespace X1bplan.PluginTesting.Plugins.Tests
{
    [TestFixture]
    public class ContactPluginTests : Helpers.PluginTestBase
    {
        private const string OPEN_TASKS_EXCEPTION = "This Contact has open Tasks associated with it.";

        [SetUp]
        public void Setup()
        {
            this.PipelineContextMock.Setup(p => p.UserId).Returns(Guid.NewGuid);
            this.PipelineContextMock.Setup(p => p.PrimaryEntityName).Returns("contact");
            this.PipelineContextMock.Setup(p => p.Stage).Returns(STAGE_PREVALIDATION);
        }

        [Test]
        public void ShouldThrowExceptionIfOpenTaskExists()
        {
            Entity contact = new Entity()
            {
                LogicalName = "contact",
                Id = Guid.NewGuid()
            };

            ParameterCollection parameters = new ParameterCollection();
            parameters["Target"] = contact.ToEntityReference();
            this.PipelineContextMock.Setup(p => p.InputParameters).Returns(parameters);
            this.PipelineContextMock.Setup(p => p.MessageName).Returns("Delete");
            EntityCollection tasks = new EntityCollection() { EntityName = "task", Entities = { GetRelatedTask(contact.Id) } };
            this.OrganizationServiceMock.Setup(s => s.RetrieveMultiple(It.IsAny<QueryExpression>())).Returns(tasks);

            PreValidateContactDelete plugin = new PreValidateContactDelete();
            var exception = Assert.Throws<InvalidPluginExecutionException>(() => plugin.Execute(ServiceProviderMock.Object));
            Assert.That(exception.Message, Is.StringContaining(OPEN_TASKS_EXCEPTION));
        }

        private Entity GetRelatedTask(Guid guid)
        {
            Entity task = new Entity("task") { Id = Guid.NewGuid() };
            task.Attributes.Add("regardingobjectid", guid);
            task.Attributes.Add("statecode", 0);
            return task;
        }
    }
}
