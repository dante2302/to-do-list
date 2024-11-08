import { useState } from "react"
import ToDoTask from "../interfaces/ToDoTask";
import Task from "./Task/Task";
import "./temp.css";

function App() {
  const [tempTask, setTempTask] = useState<ToDoTask>({
    title: "Test",
    dueDate: new Date(),
    id: 1,
    isCompleted: false
  });

  return (
    <div className="outer">
      <Task taskData={tempTask} setTaskData={setTempTask}>
      </Task>
    </div>
  )
}

export default App
