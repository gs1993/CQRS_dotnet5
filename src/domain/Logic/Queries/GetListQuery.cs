﻿using System.Collections.Generic;
using System.Linq;
using Logic.Dtos;
using Logic.Models;
using Logic.Utils;
using System.Data.SqlClient;
using Dapper;
using System.Threading.Tasks;

namespace Logic.AppServices
{
    public sealed class GetListQuery : IQuery<List<StudentDto>>
    {
        public string EnrolledIn { get; }
        public int? NumberOfCourses { get; }

        public GetListQuery(string enrolledIn, int? numberOfCourses)
        {
            EnrolledIn = enrolledIn;
            NumberOfCourses = numberOfCourses;
        }

        internal sealed class GetListQueryHandler : IAsyncQueryHandler<GetListQuery, List<StudentDto>>
        {
            private readonly QueriesConnectionString _connectionString;

            public GetListQueryHandler(QueriesConnectionString connectionString)
            {
                _connectionString = connectionString;
            }

            public async Task<List<StudentDto>> Handle(GetListQuery query)
            {
                string sql = @"
                    SELECT s.StudentID Id, s.Name, s.Email,
	                    s.FirstCourseName Course1, s.FirstCourseCredits Course1Credits, s.FirstCourseGrade Course1Grade,
	                    s.SecondCourseName Course2, s.SecondCourseCredits Course2Credits, s.SecondCourseGrade Course2Grade
                    FROM dbo.Student s
                    WHERE (s.FirstCourseName = @Course  OR s.SecondCourseName = @Course OR @Course IS NULL)
                        AND (s.NumberOfEnrollments = @Number OR @Number IS NULL)
                    ORDER BY s.StudentID ASC";

                using (SqlConnection connection = new SqlConnection(_connectionString.Value))
                {
                    var students = await connection
                        .QueryAsync<StudentDto>(sql, new
                        {
                            Course = query.EnrolledIn,
                            Number = query.NumberOfCourses
                        });

                    return students.ToList();
                }
            }
        }
    }
}
