pipeline {
  agent {
    dockerfile {
      filename 'Dockerfile'
    }

  }
  stages {
    stage('Deploy') {
      steps {
        sh 'kubectl apply -f ""'
      }
    }
  }
}