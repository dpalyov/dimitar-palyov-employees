import React, { useState } from 'react';
import {Input,  Button, Table, FormGroup, Form, FormFeedback} from "reactstrap"
import "./Home.css";

function Home() {

  const [file, setFile] = useState(null);
  const [data, setData] = useState(null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleFilePick = e => {
    const file = e.target.files[0];
    setFile(file);
  }

  const upload = async () => {


      if(file) {

        const form = new FormData();
        form.append("file", file, file.name);
        
        setLoading(true);
  
        const res = await fetch("/calculator", {
          method: "POST",
          body: form,
        });
  
  
        if(res.ok){
          console.info("Success");
          setData(await res.json());
        }
        else {
          console.error(res.statusText);
        }
  
        setLoading(false);
      }
      else {
        setError("File not selected!");
      }

  }


  return (
    <div className="home-container">
      <h1>Check best employee duo</h1>
      <Form>
        <FormGroup>
          <label className="custom-file-upload">
            <Input type="file"  onChange={handleFilePick} name="file"/>
            {file ? `Selected: ${file.name}` : "Select a file to upload"}
          </label>
          </FormGroup>
          <span style={{color: "red"}}>{error}</span>
          <Button color="primary" onClick={upload}>Upload</Button>
        </Form>
        {loading && <span>Loading...</span>}
        {data && 
        <Table borderless>
          <thead>
            <tr>
              <th>EmployeeID #1</th>
              <th>EmployeeID #2</th>
              <th>Project ID</th>
              <th>Days worked</th>
            </tr>
          </thead>
          <tbody>
            {data.map(p => {
              return (
                <tr key={p.projectId}>
                  <td>{p.employeeIdA}</td>
                  <td>{p.employeeIdB}</td>
                  <td>{p.projectId}</td>
                  <td>{p.daysOnProject}</td>
                </tr>
              )
            })}
          </tbody>
        </Table> 
        }
    </div>
  )
}

export default Home
