import { STATUS } from "../enums/Status";
import { TaskStatus } from "../enums/TaskStatus";
import { ToDoSubmission } from "../interfaces/ToDoSubmission";
import ToDoTask from "../interfaces/ToDoTask";
import * as request from "./request";
const BASE_URL = import.meta.env.VITE_API_URL;
const TODO_URL = `${BASE_URL}/todos`

export async function create(taskSubmission: ToDoSubmission)
{
    try{
        const newTask = {
            ...taskSubmission, 
            isCompleted: false
        }
        const response = await request.post(TODO_URL, newTask);
        const data: ToDoTask = await response.json();
        mapTaskDate(data);
        return {
            data,
            status: STATUS.Success
        }
    }
    catch(e)
    {
        console.log(e);
        return { 
            data: null, 
            status: STATUS.Error 
        };
    }
}
export async function getOne(id: number)
{
    try
    {
        const response = await request.get(`${TODO_URL}/${id}`);
        const data: ToDoTask = await response.json();
        mapTaskDate(data);
        return {
            data,
            status: STATUS.Success
        }
    }
    catch(e)
    {
        console.log(e);
        return { 
            data: null, 
            status: STATUS.Error 
        };
    }
}

export async function getAllByStatus(status: TaskStatus)
{
    try
    {
        const response = await request.get(`${TODO_URL}/${status}`);
        let data: ToDoTask[] = await response.json();
        data = mapTaskListDates(data); 
        return {
            data,
            status: STATUS.Success
        }
    }
    catch(e)
    {
        console.log(e);
        return { 
            status: STATUS.Error 
        };
    }
}

export async function update(updatedTodo: ToDoTask)
{
    try
    {
        const response = await request.put(`${TODO_URL}/${updatedTodo.id}`, updatedTodo);
        const data = await response.json();
        mapTaskDate(data);

        if(!response.ok)
            throw data;

        return {
            data,
            status: STATUS.Success
        }

    }
    catch(e)
    {
        console.log(e);
        return {
            data: null,
            status: STATUS.Error
        }
    }
}

export async function _delete(id: number)
{
    try
    {
        const response = await request._delete(`${BASE_URL}/todos/${id}`)
        const data = await response.json()
        if(!response.ok)
            throw data;

        return STATUS.Success
    }
    catch(e)
    {
        console.log(e);
        return STATUS.Error
    }
}

const mapTaskDate = (task: ToDoTask) => 
{
    task.dueDate = new Date(task.dueDate);
}

const mapTaskListDates = (data: ToDoTask[]) => 
{
    return data.map(task => ({ ...task, dueDate: new Date(task.dueDate) }));
}