using System;
using UnityEngine;

namespace ElephantSDK
{
    [Serializable]
    public class IapVerifyRequest : BaseData
    {
        public string package_name;
        public float local_price;
        public float price;
        public string currency;
        public string local_currency;
        public string receipt;
        public string product_id;
        public string source;

        private IapVerifyRequest(float localPrice, float price, string localCurrency, string receipt, string productId, string source)
        {
            package_name = Application.identifier;
            currency = "USD";
            local_price = localPrice;
            this.price = price;
            local_currency = localCurrency;
            this.receipt = receipt;
            product_id = productId;
            this.source = source;
        }

        public class Builder
        {
            private float _localPrice;
            private float _price;
            private string _localCurrency;
            private string _receipt;
            private string _productId;
            private string _source;

            public Builder LocalPrice(float value)
            {
                _localPrice = value;
                return this;
            }

            public Builder LocalCurrency(string value)
            {
                _localCurrency = value;
                return this;
            }
            
            public Builder Price(float value)
            {
                _price = value;
                return this;
            }
            public Builder Receipt(string value)
            {
                _receipt = value;
                return this;
            }
            public Builder ProductId(string value)
            {
                _productId = value;
                return this;
            }
            public Builder Source(string value)
            {
                _source = value;
                return this;
            }

            public IapVerifyRequest Build()
            {
                var a = new IapVerifyRequest(_localPrice, _price, _localCurrency, _receipt, _productId, _source);
                a.FillBaseData(ElephantCore.Instance.GetCurrentSession().GetSessionID());
                return a;
            }
        }
    }
}