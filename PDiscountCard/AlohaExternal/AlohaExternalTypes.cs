using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace PDiscountCard.AlohaExternal
{
    [ServiceContract(Namespace = "http://Aloha.USrv")]
    public interface IAlohaExternal
    {
        /// <summary>
        /// Возвращает список туго блюд
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
                 ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "json/GetToGoOrders"
                 )]
        GetToGoordersResponse GetToGoOrders();


        /// <summary>
        /// Возвращает список туго блюд
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
                 ResponseFormat = WebMessageFormat.Json
            , UriTemplate = "json/PrintOrder/?AlohaId={orderId}"
                 )]
        CommandResponse PrintOrder(string orderId);

        /*
        /// <summary>   
        /// Тестовый метод
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        [OperationContract]
        Person GetData(string id);
        */
        /// <summary>
        /// Добавляет блюда в существующий чек
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        [OperationContract]
        AddItemsResponse AddItems(AddItemsRequest Request);

        /// <summary>
        /// Добавляет скидки в существующий чек
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        [OperationContract]
        ApplyDiscountsResponse ApplyDiscounts(ApplyDiscountsRequest Request);

        /// <summary>
        /// Добавляет оплаты в существующий чек
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        [OperationContract]
        AddPaymentsResponse AddPayments(AddPaymentsRequest Request);

        /// <summary>
        /// Получает информацию о столе
        /// </summary>
        /// <param name="TableNumber"></param>
        /// <returns></returns>
        [OperationContract]
        AlohaTableInfoResponse GetTableInfo(int TableNumber);

        /// <summary>
        /// Получает информацию о чеке
        /// </summary>
        /// <param name="TableNumber"></param>
        /// <returns></returns>
        [OperationContract]
        AlohaCheckInfoResponse GetCheckInfo(int CheckId);

        /// <summary>
        /// Открывает стол (если данный стол не открыт),  добавляет в него чек и добавляет в чек блюда.
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        [OperationContract]
        NewOrderResponse NewOrder(NewOrderRequest Request);


        /// <summary>
        /// Открывает стол из данного диапазона,  добавляет в него чек и добавляет в чек блюда.
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        [OperationContract]
        NewOrderResponse NewOrderOnTableRange(NewOrderRequest Request);

        /// <summary>
        /// Возвращает блюда находящиеся в стоп-листе
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        GetStopListResponse GetStopList();
        /// <summary>
        /// Возвращает меню (устарело)
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        GetMenuResponse GetMenu();
        /// <summary>
        /// Актуальный метод для запроса меню
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        GetMenuResponse2 GetMenu2();

        /// <summary>
        /// В разработке
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        [OperationContract]
        CommandResponse ShowMessage(String Message);
        
        /// <summary>
        /// Возвращает текущий статус ранее полученной команды
        /// </summary>
        /// <param name="CommandId"></param>
        /// <returns></returns>
        [OperationContract]
        GetCommandStatusResponse GetCommandStatus(Guid CommandId);

    }


    public class AddItemsRequest : AddEntityRequest
    {
        /// <summary>
        /// Список товаров
        /// </summary>
        public List<AlohaItemInfo> Items { set; get; }
    }

    public class NewOrderRequest : AddEntityRequest
    {
        /// <summary>
        /// Список товаров, которые необходимо добавить в чек
        /// </summary>
        public List<AlohaItemInfo> Items { set; get; }

        /// <summary>
        /// Номер очереди. Можно не указывать.
        /// </summary>
        public int QueueId { set; get; }

        /// <summary>
        /// Количество гостей
        /// </summary>
        public int NumGuest { set; get; }

        /// <summary>
        /// Имя стола. Некий текст, который прикрепляется к столу и будет отображен на чеке. Можно не заполнять.
        /// </summary>
        public string TableName { set; get; }

        public int TableRangeId { set; get; }
        public int DiscountId { set; get; }

    }

    public class ApplyDiscountsRequest : AddEntityRequest
    {
        /// <summary>
        /// Список скидок
        /// </summary>
        public List<AlohaDiscountInfo> Discounts { set; get; }
    }
    public class AddPaymentsRequest : AddEntityRequest
    {
        /// <summary>
        /// Список оплат
        /// </summary>
        public List<AlohaPaymentInfo> Payments { set; get; }
    }

    public class AddEntityRequest : CommandRequest
    {
        /// <summary>
        /// Если указан Id чека, то блюда добавляем в него
        /// </summary>
        public int AlohaCheckId { set; get; }
        /// <summary>
        /// Если AlohaCheckId =0, то блюда добавляем в первый чек стола с этим номером
        /// </summary>
        public int TableNumber { set; get; }

        /// <summary>
        /// Внутренний Id стола в алохе. Можно не заполнять.
        /// </summary>
        public int AlohaTableId { set; get; }

    }



    public class GetMenuResponse : CommandResponse
    {
        public StopListService.AlohaMnu Mnu { set; get; }
    }
    public class GetMenuResponse2 : CommandResponse
    {
        public List<AlohaMnuExt> Mnus { set; get; }
    }

    public class GetStopListResponse : CommandResponse
    {
        /// <summary>
        /// Список баркодов в стоп-листе
        /// </summary>
        public List<int> ItemsBCs { set; get; }
    }
    public class GetCommandStatusResponse : CommandResponse
    {

        public int Code { set; get; }
    }

    public class ApplyDiscountsResponse : CommandResponse
    {
        /// <summary>
        /// Список успешно примененных скидок
        /// </summary>
        public List<AlohaDiscountInfo> AddedDiscounts = new List<AlohaDiscountInfo>();

        /// <summary>
        /// Список отклоненных скидок
        /// </summary>
        public List<AlohaDiscountInfo> ErrorDiscounts = new List<AlohaDiscountInfo>();
    }
    public class AddPaymentsResponse : CommandResponse
    {
        /// <summary>
        /// Список успешно примененных скидок
        /// </summary>
        public List<AlohaPaymentInfo> AddedDiscounts = new List<AlohaPaymentInfo>();

        /// <summary>
        /// Список оплат скидок
        /// </summary>
        public List<AlohaPaymentInfo> ErrorDiscounts = new List<AlohaPaymentInfo>();
    }

    public class AddItemsResponse : CommandResponse
    {
        /// <summary>
        /// Список успешно добавленных блюд
        /// </summary>
        public List<AlohaItemInfo> AddedItems = new List<AlohaItemInfo>();

        /// <summary>
        /// Список отклоненных блюд
        /// </summary>
        public List<AlohaItemInfo> ErrorItems = new List<AlohaItemInfo>();
    }
    public class NewOrderResponse : CommandResponse
    {
        /// <summary>
        /// Список успешно добавленных блюд
        /// </summary>
        public List<AlohaItemInfo> AddedItems = new List<AlohaItemInfo>();

        /// <summary>
        /// Список отклоненных блюд
        /// </summary>
        public List<AlohaItemInfo> ErrorItems = new List<AlohaItemInfo>();


        /// <summary>
        /// Id созданного чека
        /// </summary>
        public int CheckId { set; get; }
        /// <summary>
        /// Id созданного стола
        /// </summary>
        public int TableId { set; get; }
        /// <summary>
        /// Номер стола
        /// </summary>
        public int TableNum { set; get; }




    }

    public class CommandRequest
    {
        /// <summary>
        /// Guid он везде Guid 
        /// </summary>
        public Guid RequestId { set; get; }

        /// <summary>
        /// Это пока не используется
        /// </summary>
        public Guid TerminalNumber { set; get; }
    }

    public class CommandResponse : ICommandResponse
    {
        public Guid RequestId { set; get; }

        private bool success = true;

        /// <summary>
        /// Success или не Success
        /// </summary>
        public bool Success
        {
            set
            {
                success = value;
            }
            get
            {
                return success;
            }
        }

        /// <summary>
        /// Алоховская ошибка из-за которой команда не Success
        /// </summary>
        public AlohaErrEnum AlohaErrorCode { set; get; }

        /// <summary>
        /// Не Алоховская ошибка из-за которой команда не Success.
        /// -1 Команда с таким Guid уже отправлялась
        /// -2 Когда в GetCommandStatus приходит Guid, которого не было
        /// -3 Нет свободных столов в диапазоне
        /// </summary>
        public int IntegrationErrorCode { set; get; }

        public string ErrorMsg { set; get; }

        public int Status { set; get; }

        /// <summary>
        /// Любая сущность временная (открытвй стол, чек, блюдо, скидка, оплата итд) в Алохе имеет свой Id. Здесь его возвращаю при успешном создании сущности.
        /// </summary>
        public int AlohaId { set; get; }

    }

    public interface ICommandResponse
    {
        Guid RequestId { set; get; }
        bool Success { set; get; }
        int Status { set; get; }
        AlohaErrEnum AlohaErrorCode { set; get; }
        int IntegrationErrorCode { set; get; }
        string ErrorMsg { set; get; }
        int AlohaId { set; get; }
    }

    public class AlohaCheckInfoResponse : CommandResponse
    {

        public int CheckId { set; get; }
        public AlohaCheckInfo Check = new AlohaCheckInfo();
    }

    public class GetToGoordersResponse : CommandResponse
    {


        public List<AlohaCheckInfo> Checks = new List<AlohaCheckInfo>();
    }

    public class AlohaTableInfoResponse : CommandResponse
    {

        public int TNum { set; get; }
        public List<AlohaCheckInfo> Checks = new List<AlohaCheckInfo>();
    }

    public class AlohaCheckInfo
    {
        public int AlohaId { set; get; }
        public decimal Summ { set; get; }
        public bool IsClosed { set; get; }
        public DateTime TimeOfOpen { set; get; } 
        public DateTime? TimeOfClose { set; get; }
        public int WaiterId { set; get; }
        public String WaiterName { set; get; }
        public decimal DiscountSumm { set; get; }
        public int NumberInTable { set; get; }
        /// <summary>
        /// Id созданного чека
        /// </summary>
        public int CheckId { set; get; }
        /// <summary>
        /// Id созданного стола
        /// </summary>
        public int TableId { set; get; }

        public int TableNum { set; get; }
        

        public List<AlohaItemInfo> Dishez = new List<AlohaItemInfo>();
        public List<AlohaDiscountInfo> Comps = new List<AlohaDiscountInfo>();
        public List<AlohaPaymentInfo> Paiments = new List<AlohaPaymentInfo>();
        
    }
}
