import logo from './logo.svg';
import './App.css';
import { useAuth } from 'react-oidc-context';

function App() {
  let auth = useAuth()

  let notYetImplemented = () => alert("Not yet implemented!")
  let unsecureCall = notYetImplemented
  let acControl = notYetImplemented
  let nukeControl = notYetImplemented

  if (auth.isLoading) {
    return <div>Loading ...</div>
  }

  if (auth.error) {
    return <div>Ops, error occured {auth.error.message}</div>
  }

  if (auth.isAuthenticated) {
     console.log(auth.user)

    return (
      <>
        <button onClick={() => auth.signoutRedirect() }>Log out</button>      
        <p>Hello: { auth.user.profile.email }</p>
        <button onClick={ () => unsecureCall() }>Call Unprotected service</button>
        <button onClick={ () => acControl() }>Call Protected Service (AC Control)</button>
        <button onClick={ () => nukeControl() }>Call Protected Service (Nuke Control)</button>
      </>
    )
  }

  return (
    <>
    
      <button onClick={() => void auth.signinRedirect({scope: 'openid'}) }>Log in</button>
    </>
  )
}

export default App;
