using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TestNinja.Mocking;

namespace TestNinja.UnitTests.Mocking
{
    [TestFixture]
    public class HousekeeperServiceTests
    {
        private HousekeeperService _service;
        private Mock<IStatementGenerator> _statementGenerator;
        private Mock<IEmailSender> _emailSender;
        private Mock<IXtraMessageBox> _messageBox;
        private DateTime _statementDate = new DateTime(2017, 1, 1);
        private Housekeeper _housekeeper;
        private string _statementFileName;

        [SetUp]
        public void SetUp()
        {          
            _housekeeper = new Housekeeper { Email = "a", FullName = "b", Oid = 1, StatementEmailBody = "c"};
            
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(uow => uow.Query<Housekeeper>()).Returns(new List<Housekeeper>
            {
                _housekeeper
            }.AsQueryable());

            _statementFileName = "filename";
            _statementGenerator = new Mock<IStatementGenerator>();
            _statementGenerator
                .Setup(sg => sg.SaveStatement(_housekeeper.Oid, _housekeeper.FullName, (_statementDate)))
                .Returns(()=>_statementFileName);
            
            _emailSender = new Mock<IEmailSender>();
            
            _messageBox = new Mock<IXtraMessageBox>();
            
            _service = new HousekeeperService(
                unitOfWork.Object,
                _statementGenerator.Object,
                _emailSender.Object, 
                _messageBox.Object);
            
        }

        [Test]
        public void SendStatementEmails_WhenCalled_ShouldGenerateEmails()
        {
            _service.SendStatementEmails(_statementDate);
            
            _statementGenerator.Verify(sg => 
                sg.SaveStatement(_housekeeper.Oid, _housekeeper.FullName, _statementDate));
        }
        
        [Test]
        public void SendStatementEmails_HouseKeepersEmailIsNull_ShouldNotGenerateEmails()
        {
            _housekeeper.Email = null;
            
            _service.SendStatementEmails(_statementDate);
            
            _statementGenerator.Verify(sg => 
                sg.SaveStatement(_housekeeper.Oid, _housekeeper.FullName, (_statementDate)), Times.Never);
        }
        
        [Test]
        public void SendStatementEmails_HouseKeepersEmailIsWhitespace_ShouldNotGenerateEmails()
        {
            _housekeeper.Email = " ";
            
            _service.SendStatementEmails(_statementDate);
            
            _statementGenerator.Verify(sg => 
                sg.SaveStatement(_housekeeper.Oid, _housekeeper.FullName, (_statementDate)), Times.Never);
        }
        [Test]
        public void SendStatementEmails_HouseKeepersEmailIsEmptyString_ShouldNotGenerateEmails()
        {
            _housekeeper.Email = "";
            
            _service.SendStatementEmails(_statementDate);
            
            _statementGenerator.Verify(sg => 
                sg.SaveStatement(_housekeeper.Oid, _housekeeper.FullName, (_statementDate)), Times.Never);
        }
        
        [Test]
        public void SendStatementEmails_WhenCalled_EmailTheStatement()
        {
            _service.SendStatementEmails(_statementDate);
            
            VerifyEmailSent();
        }



        [Test]
        public void SendStatementEmails_StatementFilenameIsNull_ShouldNotEmailTheStatement()
        {
            _statementFileName = null;
            
            _service.SendStatementEmails(_statementDate);
            
            VerifyEmailNotSend();
        }



        [Test]
        public void SendStatementEmails_StatementFilenameIsEmptyString_ShouldNotEmailTheStatement()
        {
            _statementFileName = "";
            
            _service.SendStatementEmails(_statementDate);

            VerifyEmailNotSend();
        }
        
        [Test]
        public void SendStatementEmails_StatementFilenameIsWhitespace_ShouldNotEmailTheStatement()
        {
            
            _statementFileName = " ";
            
            _service.SendStatementEmails(_statementDate);

            VerifyEmailNotSend();
        }
        
        [Test]
        public void SendStatementEmails_EmailSendingFails_DisplayMsgBox()
        {
            _emailSender.Setup(es => es.EmailFile(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
                )).Throws<Exception>();
            
            _service.SendStatementEmails(_statementDate);

            _messageBox.Verify(mb => mb.Show(It.IsAny<string>(),It.IsAny<string>(), MessageBoxButtons.OK));
        }
        
        private void VerifyEmailNotSend()
        {
            _emailSender.Verify(es => es.EmailFile(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }
        
        private void VerifyEmailSent()
        {
            _emailSender.Verify(es => es.EmailFile(
                _housekeeper.Email,
                _housekeeper.StatementEmailBody,
                _statementFileName,
                It.IsAny<string>()));
        }
    }
}