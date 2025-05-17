using Crudify.Application.Dtos;

namespace Crudify.Application.Services;

public class StudentsService(ApplicationContext context) : IStudentsService
{
    public async Task<ApiResponse> GetStudentsList()
    {
        var student = await context.Students.ToListAsync();

        if (student is null || !student.Any())
        {
            return new ApiResponse
            {
                Success = false,
                Errors = ["No record found."],
            };
        }

        return new ApiResponse
        {
            Success = true,
            Payload = student
        };
    }

    public async Task<ApiResponse> GetStudent(int id)
    {
        var student = await context.Students.FindAsync(id);

        if(student is null)
        {
            return new ApiResponse
            {
                Success = false,
                Errors = ["No record found against the provided studentId."],
            };
        }

        return new ApiResponse
        {
            Success = true,
            Payload = student
        };
    }

    public async Task<ApiResponse> AddStudent(Student student)
    {
        if (student is null)
        {
            return new ApiResponse
            {
                Success = false,
                Errors = ["Invalid payload"],
            };
        }

        context.Students.Add(student);
        await context.SaveChangesAsync();

        return new ApiResponse
        {
            Success = true,
            Message = [$"{student.FullName} - added successfully"]
        };
    }

    public async Task<ApiResponse> UpdateStudent(int id, Student student)
    {
        if(id != student.Id)
        {
            return new ApiResponse
            {
                Success = false,
                Errors = ["Id MisMatch"],
            };
        }

        context.Entry(student).State = EntityState.Modified;

        await context.SaveChangesAsync();

        return new ApiResponse
        {
            Success = true,
            Message = ["Record Updated."]
        };
    }

    public async Task<ApiResponse> DeleteStudent(int id)
    {
        var student = await context.Students.FindAsync(id);

        if (student is null)
        {
            return new ApiResponse
            {
                Success = false,
                Errors = ["No record found."],
            };
        }

        context.Students.Remove(student);
        await context.SaveChangesAsync();
        return new ApiResponse
        {
            Success = true,
            Message = ["Record Deleted."]
        };
    }
}
