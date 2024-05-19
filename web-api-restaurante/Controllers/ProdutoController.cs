using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.Common;
using web_api_restaurante.Entidades;

namespace web_api_restaurante.Controllers
{
    //pegar na weather
    [ApiController]
    [Route("[controller]")]
    public class ProdutoController : ControllerBase
    {
     //para recuperar a string de conexão
        private readonly string? _connectionString;

        //ctor para criar construtor
        public ProdutoController(IConfiguration configuration)
        {
        //na conexão colocamos o que colocamos na appsettings
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //para abrir a conexao
        //DbConnection interface que auxilia a conexão c a base de dados
        private IDbConnection OpenConnection()
        {
         //passamos a string de conexão
         //interface que auxilia a conexão com base de dados
            IDbConnection dbConnection = new SqliteConnection(_connectionString);
            dbConnection.Open();
            return dbConnection;
        }



//método para recuperar produtos cadastrados, get
        [HttpGet]
        public async Task<IActionResult> Index()
        {
        //abre conexão
            using IDbConnection dbConnection = OpenConnection();
            string sql = "select id, nome, descricao, imageUrl from Produto; ";

             //armazenar resultado da conexão
            var result = await dbConnection.QueryAsync<Produto>(sql);

            return Ok(result);
        }

        //usando id como parametro
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using IDbConnection dbConnection = OpenConnection();
            string sql = "select id, nome, descricao, imageUrl from Produto where id = @id; ";

            var result = await dbConnection.QueryFirstOrDefaultAsync<Produto>(sql,  new {id});

            dbConnection.Close();

            if(result == null) 
                return NotFound();

            return Ok(result);

        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Produto produto)
        {
            using IDbConnection dbConnection = OpenConnection();
            string query = @"INSERT into Produto(nome,descricao,imageUrl)
                VALUES(@Nome, @Descricao, @ImagemUrl); ";

            await dbConnection.ExecuteAsync(query, produto);

            return Ok();
        }

        //https://spectacled-falcon-84a.notion.site/Restaurante-pedido-real-time-86b8adb9751d4706a2ce0c59e0a7ad6a
    }
}
