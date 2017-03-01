using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ManyToMany.Data;
using ManyToMany.ViewModels;
using ManyToMany.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ManyToMany.API
{
    [Route("api/[controller]")]
    public class MoviesController : Controller
    {
        private ApplicationDbContext _db;

        [HttpGet]
        public List<Movie> Get()
        {
            return _db.Movies.ToList();
        }

        [HttpGet("{id}")]
        public MovieWithActors Get(int id)
        {
            MovieWithActors mov = (from m in _db.Movies
                                   where m.Id == id
                                   select new MovieWithActors
                                   {
                                       Id = m.Id,
                                       Title = m.Title,
                                       Director = m.Director,
                                       Actors = (from ma in _db.MovieActors
                                                 where ma.MovieId == m.Id
                                                 select ma.Actor).ToList()
                                   }).FirstOrDefault();
            return mov;
        }

        [HttpPost]
        public IActionResult Post(int id, [FromBody] Movie mov)
        {
            if(mov == null)
            {
                return BadRequest();
            } else if(mov.Id == 0)
            {
                Movie tempMov = new Movie
                {
                    Title = mov.Title,
                    Director = mov.Director
                };
                _db.Movies.Add(tempMov);
                _db.SaveChanges();
                _db.MovieActors.Add(new MovieActor
                {
                    MovieId = mov.Id,
                    ActorId = mov.Id //something needs to go here...
                });
                //Movie movLookUp = (from m in _db.Movies
                //                   where mov.Title == m.Title
                //                   select m).FirstOrDefault();

                //foreach(Actor actor in mov.Actors)
                //{
                //    _db.MovieActors.Add(new MovieActor
                //    {
                //        MovieId = movLookUp.Id,
                //        ActorId = actor.Id
                //    });
                //    _db.SaveChanges();
                //}

                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        public MoviesController(ApplicationDbContext db)
        {
            _db = db;
        }
    }
}
