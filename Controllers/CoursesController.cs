using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chama.WebApi.ModelView;
using Chama.WebApi.Repositories;
using Chama.WebApi.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Chama.WebApi.Models;
using Chama.WebApi.Models.Utils;

namespace Chama.WebApi.Controllers
{
    public interface ICoursesController
    {
        IActionResult SignUp([FromBody] SignUpModelView chamaDTO);
        Task<IActionResult> SignUpAsync([FromBody] SignUpModelView chamaDTO);
        void Process();
        List<ProcessedCourseModel> GetProcessedCourses();
    }

    [Route("api/[controller]")]
    public class CoursesController : Controller, ICoursesController
    {
        private Object addLock = new Object();

        private readonly ICoursesRepository _coursesRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly ISender<SignUpModelView> _Sender;

        public CoursesController(ICoursesRepository coursesRepository, IUsersRepository usersRepository, ISender<SignUpModelView> Sender)
        {
            _coursesRepository = coursesRepository;
            _usersRepository = usersRepository;
            _Sender = Sender ?? new Sender<SignUpModelView>();
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        [Route("signUp")]
        public IActionResult SignUp([FromBody]SignUpModelView SignUpModelView)
        {
            var result = Json(new RequestResult
            {
                State = RequestState.Failed,
                Msg = "Ops, something wrong! Please check the information and try again!"
            });

            if (SignUpModelView.CourseId > 0 && !string.IsNullOrEmpty(SignUpModelView.StudentName) && SignUpModelView.StudentAge > 0)
            {
                lock (addLock)
                {
                    var course = _coursesRepository.GetById(SignUpModelView.CourseId);
                    var student = _usersRepository.RegisterOrUpdate(SignUpModelView.StudentName, SignUpModelView.StudentAge);

                    if (course != null && student != null)
                    {
                        if (course != null && course.Students.Any(c => c.Name == SignUpModelView.StudentName))
                        {
                            result = Json(new RequestResult
                            {
                                State = RequestState.Failed,
                                Msg = "Student already enrolled in this course"
                            });
                        }
                        if (course.Students == null || course.Students.Count < course.MaxStudents)
                        {
                            _coursesRepository.SignUp(course, student);


                            result = Json(new RequestResult
                            {
                                State = RequestState.Success,
                                Msg = "Student successfully enrolled"
                            });
                        }
                        else
                        {
                            result = Json(new RequestResult
                            {
                                State = RequestState.Failed,
                                Msg = "Sorry, the Course is full"
                            });
                        }
                    }
                }
            }
            return result;

        }

        [HttpPost]
        [Route("signupasync")]
        public async Task<IActionResult> SignUpAsync([FromBody]SignUpModelView SignUpModelView)
        {
            var result = Json(new RequestResult
            {
                State = RequestState.Failed,
                Msg = "Ops, something wrong! please check the information and try again!"
            });

            if (SignUpModelView.CourseId > 0 && !string.IsNullOrEmpty(SignUpModelView.StudentName) && SignUpModelView.StudentAge > 0)
            {
                await _Sender.SendAsync(SignUpModelView);

                result = Json(new RequestResult
                {
                    State = RequestState.Success,
                    Msg = "SignUp Process started successfully. Please check your e-mail."
                });
            }
            return result;
        }

        [HttpPost]
        [Route("Process")]
        public void Process()
        {
            _coursesRepository.Process();
        }

        [HttpGet]
        [Route("GetProcessedCourses")]
        public List<ProcessedCourseModel> GetProcessedCourses()
        {
            return _coursesRepository.GetProcessedCourses();
        }
    }
}
