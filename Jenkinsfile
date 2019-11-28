pipeline {
  agent {
    docker {
      image 'registry.cn-shanghai.aliyuncs.com/wzyuchen/dotnetcoresdk:3.0.100-preview5'
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