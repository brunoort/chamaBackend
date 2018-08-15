using System.Collections.Generic;
using System.Linq;
using Chama.WebApi.ContextDatabase;
using Chama.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Chama.WebApi.Repositories
{
    public interface IUsersRepository
    {
        IEnumerable<UserModel> Get();
        UserModel GetById(int id);
        UserModel RegisterOrUpdate(string name, int age);
    }

    public class UsersRepositoriy : IUsersRepository
    {
        private readonly ChamaContext _context;

        public UsersRepositoriy(ChamaContext context)
        {
            _context = context;
        }

        public UserModel RegisterOrUpdate(string name, int age)
        {
            var student = _context.Users.Where(w => w.Name == name).FirstOrDefault();

            if (student == null)
            {
                student = new UserModel();
                student.Name = name;
                student.Age = age;

                _context.Users.Add(student);
                _context.SaveChanges();
            }
            else if (student.Age != age)
            {
                student.Age = age;

                _context.Users.Attach(student);
                _context.Entry(student).State = EntityState.Modified;
                _context.SaveChanges();
            }
            return student;
        }

        public IEnumerable<UserModel> Get()
        {
            return _context.Users.ToList();
        }

        public UserModel GetById(int id)
        {
            return Get().Where(w => w.Id == id).FirstOrDefault();
        }        
    }
}
