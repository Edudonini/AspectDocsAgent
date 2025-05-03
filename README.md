
# 📚 AspectDocsAgent

> Um assistente local para responder perguntas técnicas baseadas na documentação do sistema **Aspect**, utilizando inteligência artificial embarcada.

![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet?logo=dotnet)
![Angular](https://img.shields.io/badge/Angular-17-DD0031?logo=angular)
![Ollama](https://img.shields.io/badge/Ollama-Local%20LLM-green?logo=machine-learning)
![LangChain](https://img.shields.io/badge/LangChain-.NET-yellow?logo=csharp)
![Status](https://img.shields.io/badge/status-Em%20desenvolvimento-informational)

---

## ✨ Visão Geral

O **AspectDocsAgent** é uma solução full stack que carrega arquivos PDF com a documentação interna da plataforma **Aspect** e permite realizar **perguntas em linguagem natural**, obtendo respostas precisas com base no conteúdo indexado.

> **💡 Funciona 100% offline**, com suporte a múltiplos modelos de IA locais via Ollama!

---

## ⚙️ Tecnologias Utilizadas

### Backend
- **.NET 8** (Minimal API)
- **LangChainSharp** para orquestração de LLMs
- **Ollama** como runtime local de IA (suporte a `phi3`, `deepseek`, `llama3` etc.)
- **Nomic Embed** para vetorização de textos
- **Streaming via IAsyncEnumerable** (respostas tokenizadas em tempo real)

### Frontend
- **Angular 17** com SSR (Server Side Rendering) opcional
- **TailwindCSS** para UI simples e responsiva
- Consumo da API via `HttpClient` + stream parser

---

## 🚀 Como Executar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Node.js + npm](https://nodejs.org)
- [Ollama](https://ollama.com) instalado e rodando localmente
- Modelo carregado no Ollama (ex: `phi3:mini` ou `deepseek-coder`)

### Backend (.NET)

```bash
cd src/backend
dotnet restore
dotnet run
```

### Frontend (Angular)

```bash
cd src/frontend/aspect-docs-agent-web
npm install
npm run dev
```

> Acesse o app em: http://localhost:4200  
> A API roda por padrão em: https://localhost:44376

---

## 📂 Organização

```
src/
├── backend/
│   ├── AspectDocsAgent.Api/
│   ├── AspectDocsAgent.Application/
│   ├── AspectDocsAgent.Domain/
│   └── AspectDocsAgent.Infrastructure/
└── frontend/
    └── aspect-docs-agent-web/
```

---

## 🧠 Funcionalidades

- ✅ Upload de múltiplos PDFs com parsing automático
- ✅ Vetorização dos documentos e armazenamento em memória
- ✅ Consulta com IA utilizando contexto real dos arquivos
- ✅ Suporte a resposta **streaming**
- ✅ Respostas contextualizadas com fallback
- ✅ Suporte a **múltiplos modelos LLM**
- ✅ Pronto para deploy com Ollama local

---

## 🔄 Compatibilidade com Modelos

Você pode utilizar qualquer modelo local compatível com o Ollama. Exemplos testados:

| Modelo            | Tamanho | Observações                  |
|-------------------|---------|------------------------------|
| `phi3:mini`       | 2.2 GB  | Recomendado para ambientes leves
| `deepseek-coder`  | ~9 GB   | Ideal para perguntas técnicas (code/doc)
| `llama3:instruct` | ~4 GB   | Boa qualidade geral

---

## 📦 Instalação de Modelos com Ollama

```bash
ollama pull phi3:mini
# ou
ollama pull deepseek-coder
```

---

## 🛡️ Segurança

- Nenhuma dependência externa de nuvem
- Nenhum dado sensível enviado para terceiros
- Integração 100% local

---

## 📌 TODO

- [ ] Integração com banco de dados para persistência dos vetores
- [ ] Interface de upload de arquivos
- [ ] Modo de chat contínuo com histórico
- [ ] Deploy com Docker

---

## 👨‍💻 Autor

Desenvolvido por [@Edudonini](https://github.com/Edudonini) com muito 💻 + ☕  
Este projeto faz parte do portfólio técnico focado em **.NET + Angular + IA**.

---

## 📃 Licença

MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.
