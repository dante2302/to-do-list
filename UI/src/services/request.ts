export const post = async (url: string, bodyData: object) => {
    const response = await fetch(url,{
      method: 'POST',
      headers:{
        'Content-Type':'application/json'
      },
      body: JSON.stringify(bodyData),
      mode: 'cors'
    })
    return response;
}

export const get = async (url: string) => {
  const response = await fetch(url, {
    method: 'GET',
    mode: 'cors',
    headers: {
      "Content-Type": "application/json",
    }
  })
  return response
}

export const put = async (url: string, updatedData: object) => {
  const response = await fetch(url, {
    method: "PUT",
    headers: {
      'Content-Type':'application/json',
    },
    body: JSON.stringify(updatedData)
  })
  return response;
}

export const _delete = async (url: string) => {
  const response = await fetch(url,{
    'method': 'DELETE',
    'headers':{
      "Content-Type": "application/json"
    },
  })
  return response
}