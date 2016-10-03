interface Promise<T>
{
  fromDeffered($promise);
}

Promise.prototype.fromDeffered = function<T>($promise): Promise<T> {
  return new Promise((resolve, reject) => {
    $promise.then(resolve, reject);
  });
}
