# 🛒 Sistema de Vendas Modular

Projeto de microserviços demonstrativo, focado em escalabilidade, desacoplamento e comunicação assíncrona entre serviços. Ideal para fins educacionais ou testes técnicos.

---

## 🌐 Arquitetura Geral

```

```
               🔐 Auth Service (JWT)
                      │
                      ▼
       ┌──────────────────────────┐
       │        API Gateway       │
       │      (Roteamento)       │
       └─────────┬───────────────┘
                 │
    ┌────────────┴────────────┐
    │                         │
```

┌───────────────┐          ┌───────────────┐
│ Sales Service │          │ Stock Service │
│     💰        │◄────────►│     📦        │
│    MySQL      │  RabbitMQ│    MySQL      │
└───────────────┘          └───────────────┘

```

- 🔐 **Auth Service**: Gerencia autenticação via JWT.
- 💰 **Sales Service**: Gerencia vendas e envia eventos para atualização de estoque.
- 📦 **Stock Service**: Atualiza estoque com base nos eventos do Sales Service.
- 🛠 **API Gateway**: Somente roteia requisições para os serviços corretos.

---

## 🔄 Mensageria & Filas

- Comunicação assíncrona entre **Sales** e **Stock** via **RabbitMQ**.
- Permite desacoplamento: **Sales** não depende diretamente do **Stock**.
- Facilita escalabilidade horizontal e confiabilidade do sistema.

---

## 🗄 Banco de Dados

- Cada serviço possui **MySQL próprio** e independente.
- Facilita manutenção, isolamento de dados e escalabilidade por serviço.

---

## 🛠 Tecnologias

- **.NET 7** – Backend e microserviços.
- **Ocelot** – API Gateway / roteamento centralizado.
- **Swagger for Ocelot** – Documentação unificada via Swagger.
- **RabbitMQ** – Mensageria entre serviços.
- **MySQL** – Banco de dados independente por serviço.
- **JWT** – Autenticação e autorização segura.

---

## 🚀 Como Rodar

1. Configure o **MySQL** e o **RabbitMQ** localmente ou em container.
2. Ajuste as strings de conexão de cada serviço.
3. Suba os serviços **Sales**, **Stock** e **Auth** em portas separadas.
4. Suba o **API Gateway** para roteamento centralizado.
5. Acesse a documentação unificada via **Swagger** no Gateway.

---

## 📜 Licença

MIT License - veja [LICENSE](LICENSE) para detalhes.
```