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
        private readonly string? _connectionString;

        //ctor para criar construtor
        public ProdutoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //para abrir a conexao
        //DbConnection auxilia 
        private IDbConnection OpenConnection()
        {
            IDbConnection dbConnection = new SqliteConnection(_connectionString);
            dbConnection.Open();
            return dbConnection;
        }



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using IDbConnection dbConnection = OpenConnection();
            string sql = "select id, nome, descricao, imageUrl from Produto; ";
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
