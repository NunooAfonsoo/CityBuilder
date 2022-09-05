using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Population
{
    public class Selector : CompositeTask
    {
     
        public Selector(List<Task> tasks) : base(tasks)
        {
        }

        public Selector() { }

        public override Result Run()
        {
          
            if(children.Count > this.currentChild)
            {
                Result result = children[currentChild].Run();

                if (result == Result.Running)
                    return Result.Running;

                else if(result == Result.Success)
                {
                    currentChild = 0;
                    return Result.Success;
                }
                else
                {
                    currentChild++;
                    if (children.Count > this.currentChild)
                        return Result.Running;
                    else
                    {
                        currentChild = 0;
                        return Result.Failure;
                    }
                }
            }
            return Result.Failure;
        }
    }
}
