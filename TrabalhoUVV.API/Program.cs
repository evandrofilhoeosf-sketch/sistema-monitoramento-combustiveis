using MySql.Data.MySqlClient;
using TrabalhoUVV.API.Data;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<Database>();

var app = builder.Build();

app.UseCors("Frontend");


app.MapGet("/", () =>
{
    return "API do Sistema de Monitoramento de Preços de Combustíveis funcionando!";
});


app.MapGet("/api/teste", () =>
{
    return new
    {
        mensagem = "API funcionando!",
        status = "OK"
    };
});


app.MapGet("/api/teste-banco", async (Database database) =>
{
    try
    {
        using var conexao = database.GetConnection();

        await conexao.OpenAsync();

        return Results.Ok(new
        {
            mensagem = "Conexão com o banco realizada com sucesso!",
            banco = "trabalho_uvv"
        });
    }
    catch (Exception erro)
    {
        return Results.Problem(
            detail: erro.Message,
            title: "Erro ao conectar com o banco"
        );
    }
});


app.MapGet("/api/postos", async (Database database) =>
{
    try
    {
        using var conexao = database.GetConnection();

        await conexao.OpenAsync();

        var postos = new List<object>();

        string sql = @"
            SELECT
                id_posto,
                nome_posto,
                cnpj_posto
            FROM posto
            ORDER BY nome_posto;
        ";

        using var comando = new MySqlCommand(sql, conexao);
        using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            postos.Add(new
            {
                idPosto = Convert.ToInt32(leitor["id_posto"]),
                nomePosto = Convert.ToString(leitor["nome_posto"]),
                cnpjPosto = Convert.ToString(leitor["cnpj_posto"])
            });
        }

        return Results.Ok(postos);
    }
    catch (Exception erro)
    {
        return Results.Problem(
            detail: erro.Message,
            title: "Erro ao consultar os postos"
        );
    }
});


app.MapGet("/api/combustiveis", async (Database database) =>
{
    try
    {
        using var conexao = database.GetConnection();

        await conexao.OpenAsync();

        var combustiveis = new List<object>();

        string sql = @"
            SELECT
                id_combustivel,
                nome_combustivel
            FROM combustivel
            ORDER BY nome_combustivel;
        ";

        using var comando = new MySqlCommand(sql, conexao);
        using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            combustiveis.Add(new
            {
                idCombustivel = Convert.ToInt32(leitor["id_combustivel"]),
                nomeCombustivel = Convert.ToString(leitor["nome_combustivel"])
            });
        }

        return Results.Ok(combustiveis);
    }
    catch (Exception erro)
    {
        return Results.Problem(
            detail: erro.Message,
            title: "Erro ao consultar os combustíveis"
        );
    }
});


app.MapGet("/api/verificacoes", async (Database database) =>
{
    try
    {
        using var conexao = database.GetConnection();

        await conexao.OpenAsync();

        var verificacoes = new List<object>();

        string sql = @"
    SELECT
        v.id_verificacao,
        p.id_posto,
        p.nome_posto,
        p.cnpj_posto,
        e.bairro,
        e.cidade,
        e.estado,
        e.pais,
        c.id_combustivel,
        c.nome_combustivel,
        v.preco_verificado,
        v.data_verificacao
    FROM verificacao_preco v
    INNER JOIN posto p
        ON p.id_posto = v.id_posto
    INNER JOIN endereco e
        ON e.id_posto = p.id_posto
    INNER JOIN combustivel c
        ON c.id_combustivel = v.id_combustivel
    ORDER BY v.data_verificacao, p.nome_posto;
";

        using var comando = new MySqlCommand(sql, conexao);
        using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            verificacoes.Add(new
            {
                idVerificacao = Convert.ToInt32(leitor["id_verificacao"]),

                idPosto = Convert.ToInt32(leitor["id_posto"]),

                nomePosto = Convert.ToString(leitor["nome_posto"]),

                cnpjPosto = Convert.ToString(leitor["cnpj_posto"]),

                bairro = Convert.ToString(leitor["bairro"]),

                cidade = Convert.ToString(leitor["cidade"]),

                estado = Convert.ToString(leitor["estado"]),

                pais = Convert.ToString(leitor["pais"]),

                idCombustivel = Convert.ToInt32(leitor["id_combustivel"]),

                nomeCombustivel = Convert.ToString(leitor["nome_combustivel"]),

                precoVerificado = Convert.ToDecimal(leitor["preco_verificado"]),

                dataVerificacao = Convert.ToDateTime(leitor["data_verificacao"])
            });
        }

        return Results.Ok(verificacoes);
    }
    catch (Exception erro)
    {
        return Results.Problem(
            detail: erro.Message,
            title: "Erro ao consultar as verificações"
        );
    }
});


app.Run();