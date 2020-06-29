using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;

namespace PDiscountCard.AlohaExternal
{
    /*
    class AlohaExternalJSON : IAlohaExternal
    {


        public AddItemsResponse AddItems(AddItemsRequest Request)
        {
            throw new NotImplementedException();
        }

        public ApplyDiscountsResponse ApplyDiscounts(ApplyDiscountsRequest Request)
        {
            throw new NotImplementedException();
        }

        public AddPaymentsResponse AddPayments(AddPaymentsRequest Request)
        {
            throw new NotImplementedException();
        }

        public AlohaTableInfoResponse GetTableInfo(int TableNumber)
        {
            throw new NotImplementedException();
        }

        public AlohaCheckInfoResponse GetCheckInfo(int CheckId)
        {
            throw new NotImplementedException();
        }

        public NewOrderResponse NewOrder(NewOrderRequest Request)
        {
            throw new NotImplementedException();
        }

        public NewOrderResponse NewOrderOnTableRange(NewOrderRequest Request)
        {
            throw new NotImplementedException();
        }

        public GetStopListResponse GetStopList()
        {
            throw new NotImplementedException();
        }

        public GetMenuResponse GetMenu()
        {
            throw new NotImplementedException();
        }

        public GetMenuResponse2 GetMenu2()
        {
            throw new NotImplementedException();
        }

        public CommandResponse ShowMessage(string Message)
        {
            throw new NotImplementedException();
        }

        public GetCommandStatusResponse GetCommandStatus(Guid CommandId)
        {
            throw new NotImplementedException();
        }
        
        [WebInvoke(Method = "GET",
                    ResponseFormat = WebMessageFormat.Json
                    //,UriTemplate = "data/{id}"
                    )]
        public GetToGoordersResponse GetToGoOrders()
        {
            var res = new GetToGoordersResponse();
            res.Checks = AlohaTSClass.GetToGoOrdersExternal();
            return res;
        }



        [WebInvoke(Method = "GET",
                   ResponseFormat = WebMessageFormat.Json,
                   UriTemplate = "data/{id}")]
        public Person GetData(string id)
        {
            // lookup person with the requested id 
            return new Person()
            {
                Id = Convert.ToInt32(id),
                Name = "Leo Messi"
            };
        }

       
    }
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
     * */
}
