using System;
using MediatR;
using Persistence;
using Domain;

namespace Application.Activities.Queries;

public class GetActivityDetails
{
    public class Query : IRequest<Activity> 
    {
        public required string Id { get; set; }
    }

    public class Handler(AppDbContext context) : IRequestHandler<Query, Activity>
    {
        public async Task<Activity> Handle(Query request, CancellationToken cancellationToken)
        {
            // The ! tells the compiler we expect a result, fixing the null warning
            var activity = await context.Activities.FindAsync([request.Id], cancellationToken);
            return activity!; 
        }
    }
}