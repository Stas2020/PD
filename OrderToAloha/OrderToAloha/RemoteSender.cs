using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataReciver
{
    [Serializable]
    public class STItem
    {
        public STItem()
        { }
        public int Barcode { set; get; }
        public int SourceBase { set; get; }
        public decimal Price { set; get; }
        public int Count { set; get; }
        public string Name { set; get; }    
    }

    [Serializable]
    public class STCommand
    {

        public STCommand()
        {
            Id = new Guid();
        }
        public Guid Id { set; get; }

        public STCommandType CommandType { set; get; }
        /*
        public int OrderId { set; get; }
        public List<STItem> Items { set; get; }
        public int CompanyId { set; get; }
        public string CompanyName { set; get; }
        public string BortName { set; get; }
        public int DiscountId { set; get; }
        public int Margin { set; get; }
        public DateTime TimeOfShipping { set; get; }
        public int AlohaCheckId { set; get; }
        public int AlohaTableNum { set; get; }
        */

        public SendOrderToAlohaRequest sendOrderToAlohaRequest { set; get; }
        public DeleteOrderRequest deleteOrderRequest { set; get; }
        public bool Result { set; get; }
        public int ResultId { set; get; }
        public string Sender { set; get; }
        public int SenderPort { set; get; }
        public bool Ansver { set; get; }
        public string ExeptionMessage { set; get; }



        //  public List<Item> OrderBody = new List<Item>();

    }
    [Serializable]
    public enum STCommandType
    {
        AddOrder, DeleteOrder
    }

    [Serializable]
    public class SendOrderToAlohaRequest : OrderRequest
    {
        public SendOrderToAlohaRequest()
        { }

        /// <summary>
        /// Уникальный номер заказа в СТ. По этому номеру происходит идентификация заказа при его изменении. 
        /// </summary>
        public int OrderId { set; get; }

        /// <summary>
        /// Список блюд в заказе
        /// </summary>
        public List<OrderToAloha.Item> Items { set; get; }

        /// <summary>
        /// Номер компании для которой формируется заказ. 
        /// </summary>
        public int CompanyId { set; get; }

        /// <summary>
        /// Имя компании
        /// </summary>
        public string CompanyName { set; get; }

        /// <summary>
        /// Номер борта
        /// </summary>
        public string BortName { set; get; }
        /// <summary>
        /// Идентификатор скидки
        ///0 –нет скидки;
        ///1 – скидка 5%;
        ///2 – скидка 10%;
        /// </summary>
        public int DiscountId { set; get; }



        /// <summary>
        /// Велечина скидки
        
        /// </summary>
        public int FreeDisc { set; get; }

        /// <summary>
        /// Идентификатор – наценки
        ///	0 – нет наценок;
        ///2 – наценка 10%;
        /// </summary>
        public int Margin { set; get; }

        /// <summary>
        /// Время отгрузки заказа
        /// </summary>
        public DateTime TimeOfShipping { set; get; }

        /// <summary>
        /// Номер заказа в Алохе
        /// </summary>
        public int AlohaCheckId { set; get; }

        /// <summary>
        /// Номер стола в Алохе
        /// </summary>
        public int AlohaTableNum { set; get; }
    }



    [Serializable]
    public class AlohaResponse : OrderRequest
    {
        public AlohaResponse()
        { }

        /// <summary>
        /// Номер заказа в СТ
        /// </summary>
        public int OrderId { set; get; }
        public STCommandType CommandType { set; get; }
        
        /// <summary>
        /// Результат. Если 1, то все хорошо
        /// </summary>
        public int ResultId { set; get; }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string Err { set; get; }
        /// <summary>
        /// Номер чека в алохе
        /// </summary>
        public int AlohaCheckId { set; get; }
        /// <summary>
        /// Номер стола в алохе
        /// </summary>
        public int AlohaTableNum { set; get; }
    }



    [Serializable]
    public class DeleteOrderRequest : OrderRequest
    {
        public DeleteOrderRequest()
        { }
        public int OrderId { set; get; }
    }

    [Serializable]
    public class OrderRequest
    {
        public OrderRequest()
        { }
        /// <summary>
        /// Имя компьютера с Алохой
        /// </summary>
        public string RemoteCompName { set; get; }
        /// <summary>
        /// Порт
        /// </summary>
        public int port { set; get; }
    }
}
