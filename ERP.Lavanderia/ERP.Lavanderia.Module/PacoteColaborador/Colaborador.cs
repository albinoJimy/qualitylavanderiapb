using System;
using System.ComponentModel;

using DevExpress.Xpo;
using DevExpress.Data.Filtering;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.ConditionalEditorState;
using ERP.Lavanderia.Module.RecursosHumanos;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Xpo.Metadata;
using ERP.Lavanderia.Module.PacotePessoa;
using ERP.Lavanderia.Module.PacoteRecursosHumanos;
using ERP.Lavanderia.Module.PacoteEmpresa;
using ERP.Lavanderia.Module.PacoteGeral;
using ERP.Lavanderia.Module.PacoteSeguranca;
using ERP.Lavanderia.Module.PacoteCaixa;

namespace ERP.Lavanderia.Module.PacoteColaborador
{
    /// <summary>
    /// Classe utilizada para manter os colaboradores (funcion�rios).
    /// </summary>
    [DefaultClassOptions]
    [DefaultProperty("Pessoa.Nome")]
    [EditorStateRuleAttribute("DesativaDadosAdmitido", "DataDemissao", EditorState.Disabled, "Matricula.Situacao != 4", ViewType.DetailView)]
    [EditorStateRuleAttribute("Colaborador.DesativaUltimaAlteracao", "DataUltimaAtualizacao", EditorState.Disabled, "true", ViewType.DetailView)]
    public class Colaborador : BaseObject
    {
        private Pessoa pessoa;
        private DateTime dataCadastro;
        private DateTime dataUltimaAtualizacao;
        private ColaboradorCargo cargo;
        private Matricula matricula;
        private string observacoes;
        private Departamento departamento;
        private bool desativapainel;
        private DateTime dataAdmissao;
        private DateTime? dataDemissao;
        private Usuario usuario;
        private string senha;
        private string nomeDeUsuario;
        private PapelColaborador papel;

        public Colaborador(Session session) : base(session) { }

        [RuleFromBoolProperty("RuleFromBoolProperty.Colaborador.PessoaUnicaPorEmpresa", DefaultContexts.Save,
    CustomMessageTemplate = "Esta pessoa j� est� registrada como colaborador.")]
        private bool PessoaUnicaPorEmpresa
        {
            get
            {
                try
                {
                    Colaborador col = RetornaColaboradorPorPessoaEEmpresa(Pessoa, Departamento.Empresa, Session);
                    return col == null || col.Equals(this);
                }
                catch {
                    return false;
                }
            }
        }

        [RuleRequiredField("RuleRequiredField Colaborador.Pessoa", DefaultContexts.Save)]
        [DataSourceCriteria("TipoPessoa=0")]
        [ImmediatePostData]
        [Association("Pessoa-Colaborador", typeof(Pessoa))]
        [ExpandObjectMembers(ExpandObjectMembers.InListView)]
        public Pessoa Pessoa
        {
            get { return pessoa; }
            set
            {
                SetPropertyValue("Pessoa", ref pessoa, value);
            }
        }

        [RuleRequiredField("RuleRequiredField Colaborador.Papel", DefaultContexts.Save)]
        public PapelColaborador Papel
        {
            get { return papel; }
            set {
                SetPropertyValue("Papel", ref papel, value);
            }
        }

        [NonPersistent]
        [System.ComponentModel.Browsable(false)]
        [RuleFromBoolProperty("Colaborador.PessoaValida", DefaultContexts.Save, @"Colaborador deve ser pessoa f�sica.")]
        public bool PessoaValida
        {
            get
            {
                if (pessoa != null)
                {
                    if (pessoa.TipoPessoa == TipoPessoa.Fisica)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        [RuleRequiredField("Colaborador.RuleRequiredField.Senha", DefaultContexts.Save)]
        public string Senha
        {
            get { return senha; }
            set {
                SetPropertyValue("Senha", ref senha, value);
            }
        }

        [RuleRequiredField("Colaborador.RuleRequiredField.NomeDeUsuario", DefaultContexts.Save)]
        public string NomeDeUsuario
        {
            get { return nomeDeUsuario; }
            set
            {
                SetPropertyValue("NomeDeUsuario", ref nomeDeUsuario, value);
            }
        }

        [RuleUniqueValue("Colaborador.MatriculaUnica", DefaultContexts.Save, @"""Matr�cula"" j� cadastrada a outro colaborador.")]
        [RuleRequiredField("Colaborador.RuleRequiredField.Matricula", DefaultContexts.Save)]
        public Matricula Matricula
        {
            get
            {
                return matricula;
            }
            set
            {
                BaseObjectExtension.SetPropertyValue1to1(this, "Matricula", ref matricula, value, "Colaborador");
            }
        }

        [RuleRequiredField("RuleRequiredField Colaborador.DataCadastro", DefaultContexts.Save)]
        public DateTime DataCadastro
        {
            get { return dataCadastro; }
            set
            {
                SetPropertyValue("DataCadastro", ref dataCadastro, value);
            }
        }

        public DateTime DataUltimaAtualizacao
        {
            get
            {
                return dataUltimaAtualizacao;
            }
            set
            {
                SetPropertyValue("DataUltimaAtualizacao", ref dataUltimaAtualizacao, value);
            }
        }

        /// <summary>
        /// Define o cargo do colaborador.
        /// </summary>
        [ExpandObjectMembers(ExpandObjectMembers.Never)]
        public ColaboradorCargo ColaboradorCargo
        {
            get { return cargo; }
            set
            {
                SetPropertyValue("ColaboradorCargo", ref cargo, value);
            }
        }

        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [Size(4096)]
        public string Observacoes
        {
            get { return observacoes; }
            set
            {
                SetPropertyValue("Observacoes", ref observacoes, value);
            }
        }

        [RuleRequiredField("RuleRequiredField.Colaborador.Departamento", DefaultContexts.Save)]
        public Departamento Departamento
        {
            get { return departamento; }
            set
            {
                SetPropertyValue("Departamento", ref departamento, value);
            }
        }

        public bool DesativarPainelColaborador
        {
            get { return desativapainel; }
            set { SetPropertyValue("DesativarPainelColaborador", ref desativapainel, value); }
        }

        [RuleRequiredField("Colaborador.RuleRequiredField.DataAdmissao", DefaultContexts.Save)]
        public DateTime DataAdmissao
        {
            get { return dataAdmissao; }
            set { SetPropertyValue("DataAdmissao", ref dataAdmissao, value); }
        }

        public DateTime? DataDemissao
        {
            get { return dataDemissao; }
            set { SetPropertyValue("DataDemissao", ref dataDemissao, value); }
        }

        [Browsable(false)]
        [RuleUniqueValue("Colaborador.Usuario", DefaultContexts.Save, @"""Usu�rio"" j� cadastrada a outro colaborador.")]
        [RuleRequiredField("Colaborador.RuleRequiredField.Usuario", DefaultContexts.Save)]
        public Usuario Usuario
        {
            get { return usuario; }
            set { SetPropertyValue("Usuario", ref usuario, value); }
        }

        [RuleFromBoolProperty("Colaborador.RuleFromBoolProperty.ValidaUsuario", DefaultContexts.Save,
CustomMessageTemplate = "J� existe uma pessoa com esse nome de usu�rio")]
        [Browsable(false)]
        public bool ValidaUsuario
        {
            get
            {
                var user = Usuario.RetornaUsuarioPorNomeDeUsuario(Session, NomeDeUsuario);

                return user == null || user.Equals(Usuario);
            }
        }

        [Association("Colaborador-MovimentacaoCaixa")]
        public XPCollection<MovimentacaoCaixa> Pagamentos
        {
            get { return GetCollection<MovimentacaoCaixa>("Pagamentos"); }
        }

        #region Modulo Auditoria
        //M�dulo de auditoria
        private ReadOnlyCollection<AuditDataItemPersistent> changeHistory;
        public ReadOnlyCollection<AuditDataItemPersistent> ChangeHistory
        {
            get
            {
                if (changeHistory == null)
                {
                    IList<AuditDataItemPersistent> sourceCollection;
                    sourceCollection = AuditedObjectWeakReference.GetAuditTrail(Session, this);
                    if (sourceCollection == null)
                    {
                        sourceCollection = new List<AuditDataItemPersistent>();
                    }
                    changeHistory = new ReadOnlyCollection<AuditDataItemPersistent>(sourceCollection);
                }
                return changeHistory;
            }
        }
        #endregion

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            //Define o valor padr�o da data de cadastro
            dataCadastro = System.DateTime.Now;
            Papel = PacoteColaborador.PapelColaborador.Nivel1;

            Usuario = new Usuario(Session);
            Usuario.UserName = DateTime.Now.ToString();
        }

        protected override void OnSaving()
        {
            this.DataUltimaAtualizacao = DateTime.Now;

            if (Usuario != null)
            {
                Usuario.SetPassword(Senha);
                Usuario.UserName = NomeDeUsuario;

                TipoPapelLavanderia tipo = TipoPapelLavanderia.FuncionarioNivel1;

                switch (Papel) {
                    case PacoteColaborador.PapelColaborador.Nivel1: {
                        tipo = TipoPapelLavanderia.FuncionarioNivel1;
                    }; break;
                    case PacoteColaborador.PapelColaborador.Nivel2:
                    {
                        tipo = TipoPapelLavanderia.FuncionarioNivel2;
                    }; break;
                    case PacoteColaborador.PapelColaborador.Gerente:
                    {
                        tipo = TipoPapelLavanderia.Gerente;
                    }; break;
                }

                Papel papelEscolhido = ERP.Lavanderia.Module.PacoteSeguranca.Papel.RetornaPapel(tipo, Session);
                Usuario.Roles.Add(papelEscolhido);
            }

            if (Matricula != null) {
                Matricula.Colaborador = this;
            }

            base.OnSaving();
        }

        [NonPersistent, Browsable(false)]
        public bool IsNew
        {
            get
            {
                return Session.IsNewObject(this);
            }
        }

        [RuleFromBoolProperty("Colaborador.RuleFromBoolProperty.ValidaDeletar", DefaultContexts.Delete,
            CustomMessageTemplate = "Esse colaborador possui associa��es e n�o pode ser deletado")]
        [Browsable(false)]
        public bool ValidaDeletar
        {
            get
            {
                ICollection objs = Session.CollectReferencingObjects(this);
                if (objs.Count > 0)
                {
                    foreach (XPMemberInfo mi in ClassInfo.CollectionProperties)
                    {
                        //if (mi.IsAggregated && mi.IsCollection && mi.IsAssociation)
                        if (mi.IsCollection && mi.IsAssociation)
                        {
                            foreach (IXPObject obj in objs)
                            {
                                if (((XPBaseCollection)mi.GetValue(this)).BaseIndexOf(obj) < 0)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }

                return true;
            }
        }

        #region Metodos

        public static XPCollection RetornaColaboradoresComPIS(Session session, Guid oidEmpresa)
        {
            XPCollection colaboradores = new XPCollection(typeof(Colaborador),
                new OperandProperty("Matricula.Empresa.Oid") == new OperandValue(oidEmpresa) &
                !new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, new OperandProperty("Pessoa.Pis")));

            return colaboradores;
        }

        public static XPCollection RetornaColaboradoresComPIS(Session session, Empresa empresa)
        {
            XPCollection colaboradores = new XPCollection(typeof(Colaborador),
                new OperandProperty("Matricula.Empresa") == new OperandValue(empresa) &
                !new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, new OperandProperty("Pessoa.Pis")));

            return colaboradores;
        }

        public static XPCollection RetornaColaboradoresComPIS(IObjectSpace objectSpace, Empresa empresa)
        {
            XPCollection colaboradores = objectSpace.CreateCollection(typeof(Colaborador),
                new OperandProperty("Matricula.Empresa") == new OperandValue(empresa) &
                !new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, new OperandProperty("Pessoa.Pis")))
                as XPCollection;

            return colaboradores;
        }

        public static XPCollection RetornaColaboradores(IObjectSpace objectSpace, Empresa empresa)
        {
            XPCollection colaboradores = objectSpace.CreateServerCollection(typeof(Colaborador), new BinaryOperator("Matricula.Empresa", empresa)) as XPCollection;

            if (colaboradores != null)
            {
                colaboradores.DisplayableProperties = "Oid;Pessoa.Nome";
            }

            return colaboradores;
        }

        public static XPCollection RetornaColaboradoresPorDepartamento(IObjectSpace objectSpace, Departamento departamento)
        {
            return objectSpace.CreateServerCollection(typeof(Colaborador), new BinaryOperator("Departamento", departamento)) as XPCollection;
        }

        public static XPCollection RetornaColaboradoresPorDepartamento(Session session, Departamento departamento)
        {
            return new XPCollection(session, typeof(Colaborador), new BinaryOperator("Departamento", departamento));
        }

        public static Colaborador RetornaColaboradorPorMatricula(Matricula matricula, IObjectSpace objectSpace)
        {
            return objectSpace.FindObject<Colaborador>(new OperandProperty("Matricula") == matricula);
        }

        public static Colaborador RetornaColaboradorPorMatricula(Matricula matricula, Session session)
        {
            return session.FindObject<Colaborador>(new OperandProperty("Matricula") == matricula);
        }

        public static Colaborador RetornaColaboradorPorPessoaEEmpresa(Pessoa pessoa, Empresa empresa, Session session)
        {
            return session.FindObject<Colaborador>(new OperandProperty("Pessoa") == pessoa
                & new OperandProperty("Departamento.Empresa") == empresa);
        }

        public static Colaborador RetornaColaboradorLogado(ObjectSpace space) {
            return space.FindObject<Colaborador>(new BinaryOperator("Usuario", Usuario.RetornaUsuarioLogado(space)));
        }

        public static Colaborador RetornaColaboradorLogado(Session session)
        {
            return session.FindObject<Colaborador>(new BinaryOperator("Usuario", Usuario.RetornaUsuarioLogado(session)));
        }

        #endregion
    }

    public enum PapelColaborador { 
        Nivel1, Nivel2, Gerente
    }

}
