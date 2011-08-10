using System;
using System.ComponentModel;

using DevExpress.Xpo;
using DevExpress.Data.Filtering;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using ERP.Lavanderia.Module.PacoteColaborador;
using ERP.Lavanderia.Module.PacoteConfiguracoes;

namespace ERP.Lavanderia.Module.PacoteCaixa
{
    [DefaultProperty("ToStringProperty")]
    [DefaultClassOptions]
    public class MovimentacaoCaixa : BaseObject
    {
        private DateTime data;
        private string observacoes;
        private Modo modo;
        private Capital capital;
        private Caixa caixa;
        private TipoMovimentacao tipo;
        private decimal valor;

        public MovimentacaoCaixa(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            // if (!IsLoading){
            //    It is now OK to place your initialization code here.
            // }
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.

            Data = DateTime.Now;
            Caixa = ConfiguracaoGeral.RetornaConfiguracaoGeral(Session).CaixaPadrao;
        }

        [RuleRequiredField("RuleRequiredField MovimentacaoCaixa.Caixa", DefaultContexts.Save)]
        public Caixa Caixa
        {
            get { return caixa; }
            set { SetPropertyValue("Caixa", ref caixa, value); }
        }

        [RuleRequiredField("RuleRequiredField MovimentacaoCaixa.Data", DefaultContexts.Save)]
        public DateTime Data
        {
            get { return data; }
            set { SetPropertyValue("Data", ref data, value); }
        }

        [RuleRequiredField("RuleRequiredField MovimentacaoCaixa.Modo", DefaultContexts.Save)]
        public Modo Modo
        {
            get { return modo; }
            set { SetPropertyValue("Modo", ref modo, value); }
        }

        [RuleRequiredField("RuleRequiredField MovimentacaoCaixa.Tipo", DefaultContexts.Save)]
        public TipoMovimentacao Tipo
        {
            get { return tipo; }
            set { SetPropertyValue("Tipo", ref tipo, value); }
        }

        [RuleRequiredField("RuleRequiredField MovimentacaoCaixa.Capital", DefaultContexts.Save)]
        public Capital Capital
        {
            get { return capital; }
            set { SetPropertyValue("Capital", ref capital, value); }
        }

        [RuleRequiredField("RuleRequiredField MovimentacaoCaixa.Valor", DefaultContexts.Save)]
        public decimal Valor
        {
            get { return valor; }
            set { SetPropertyValue("Valor", ref valor, value); }
        }

        public string Observacoes
        {
            get { return observacoes; }
            set { SetPropertyValue("Observacoes", ref observacoes, value); }
        }

        [Browsable(false)]
        [Association("Colaborador-MovimentacaoCaixa", typeof(Colaborador))]
        public XPCollection Colaboradores
        {
            get { return GetCollection("Colaboradores"); }
        }

        [Browsable(false)]
        [NonPersistent]
        public string ToStringProperty {
            get {
                try
                {
                    return "(" + Caixa.Nome + ") " + Data + "; " + Modo + "; " +
                        Tipo + "; " + Capital + "; " + Observacoes;
                }
                catch {
                    return "N�o foi poss�vel exibir a descri��o";
                }
            }
        }

        [NonPersistent]
        [System.ComponentModel.Browsable(false)]
        [RuleFromBoolProperty("MovimentacaoCaixa.ValorMaiorOuIgualAZero", DefaultContexts.Save, @"""Valor"" deve ser maior ou igual a zero.")]
        public bool QuantidadeMaiorQueZero
        {
            get
            {
                return Valor >= 0;
            }
        }
    }

    public enum Modo { 
        Entrada, Saida
    }

}
