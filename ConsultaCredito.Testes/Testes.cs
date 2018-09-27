﻿using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ConsultaCredito.Testes
{
    public class Testes
    {
        private const string CPF_INVALIDO = "123A";
        private const string CPF_ERRO_COMUNICACAO = "76217486300";
        private const string CPF_SEM_PENDENCIAS = "60487583752";
        private const string CPF_INADIMPLENTE = "82226651209";

        private IServicoConsultaCredito mock;

        public Testes()
        {
            mock = Substitute.For<IServicoConsultaCredito>();

            mock.ConsultarPendenciaPorCPF(CPF_INVALIDO).Returns((List<Pendencia>)null);

            mock.ConsultarPendenciaPorCPF(CPF_ERRO_COMUNICACAO).Returns(s => { throw new Exception("Erro de comunicação..."); });

            mock.ConsultarPendenciaPorCPF(CPF_SEM_PENDENCIAS).Returns(new List<Pendencia>());

            Pendencia pendencia = new Pendencia();
            pendencia.CPF = CPF_INADIMPLENTE;
            pendencia.NomePessoa = "joão da Silva";
            pendencia.NomeReclamante = "Banco Itaú";
            pendencia.DescricaoPendencia = "Parcelas não pagas";
            pendencia.VLPendencia = 700;

            List<Pendencia> pendencias = new List<Pendencia>();
            pendencias.Add(pendencia);

            mock.ConsultarPendenciaPorCPF(CPF_INADIMPLENTE).Returns(pendencias);
        }

        private StatusConsultaCredito ObterStatusAnaliseCredito(string cpf)
        {
            AnaliseCredito analise = new AnaliseCredito(mock);
            return analise.ConsultarSituacaoCPF(cpf);
        }

        [Fact]
        public void TestarParametroInvalido()
        {
            StatusConsultaCredito status = ObterStatusAnaliseCredito(CPF_INVALIDO);
            Assert.Equal(StatusConsultaCredito.ParametroEnvioInvalido, status);
        }

        [Fact]
        public void TestarErroComunicacao()
        {
            StatusConsultaCredito status = ObterStatusAnaliseCredito(CPF_ERRO_COMUNICACAO);
            Assert.Equal(StatusConsultaCredito.ErroComunicacao, status);
        }

        [Fact]
        public void TestarCPFSemPendencias()
        {
            StatusConsultaCredito status = ObterStatusAnaliseCredito(CPF_SEM_PENDENCIAS);
            Assert.Equal(StatusConsultaCredito.SemPendencia, status);
        }

        [Fact]
        public void TestarCPFInadimplente()
        {
            StatusConsultaCredito status = ObterStatusAnaliseCredito(CPF_INADIMPLENTE);
            Assert.Equal(StatusConsultaCredito.Inadimplente, status);
        }
    }
}
