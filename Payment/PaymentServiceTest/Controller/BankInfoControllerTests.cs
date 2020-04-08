using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using PaymentService.Controllers;
using PaymentService.Data.Model;
using PaymentService.Infrastructure.Contracts;
using PaymentService.Services.Contracts;
using Read = PaymentService.Model.ReadModel;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace PaymentServiceTest
{
    public class BankInfoControllerTests
    {
        [Fact]
        public async Task Get_ReturnsABankInfoEntity_WhenIdIsNotNull()
        {

            // Arrange
            var input = 1;
            var mockLogger = new Mock<IApplicationLogger<BankInfoController>>(); 
            var mockBankInfoService = new Mock<IBankInfoService>();
            var result = new Read.BankInfo()
                {
                    Id = 1,
                    BankCode = "HDFC",
                    Url = "www.hdfc.com"
                };


             mockBankInfoService.Setup(svc => svc.GetBankInfoByIdAsync(input))
                .Returns(Task.FromResult(result));

            var controller = new BankInfoController(mockLogger.Object, mockBankInfoService.Object);

            //Act
            var response = await controller.GetBankInfoAsync(input);


            //Assert

          var bankInfoResult = Assert.IsType<ActionResult<Read.BankInfo>>(response);
          Assert.Equal("HDFC", bankInfoResult.Value.BankCode);


            
            
        }

    private List<BankInfo> GetTestBankInfo()
    {
        var bankInfoList = new List<BankInfo>();
        bankInfoList.Add(new BankInfo()
        {
            Id = 1,
            BankCode = "HDFC",
            Url = "www.hdfc.com"
        });

        bankInfoList.Add(new BankInfo()
        {
            Id = 2,
            BankCode = "ICICI",
            Url = "www.icici.com"
        });
        


        
        return bankInfoList;
   
   
   
    }
    

     private BankInfo GetTestBankInfo(int id)
    {
        var bankInfoList = GetTestBankInfo();
        var result = bankInfoList.Find(input =>  input.Equals(id));
        


        
        return result;
    }
   
    
    
    
    }
}   
