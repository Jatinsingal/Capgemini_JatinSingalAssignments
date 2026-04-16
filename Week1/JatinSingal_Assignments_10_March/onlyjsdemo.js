 function test() {
    console.log("Hello world");
}

function test2(num1, num2) {
    return (num1 + num2);
}


test();
let sum = test2(12, 45);
console.log(sum);
//using arrow functions
const testme = () => console.log("Hello world2");
let sum2 = (n1, n2) => (n1 + n2);
testme();
console.log(sum2(12, 56));

//variable.map(element)=>print(element)

var arr = [10, 20, 30, 40, 50];
arr.map((ele) => console.log(ele));

// example 2 
const numbers = [1, 2, 3, 4, 5];
const squares = numbers.map(value => value * value);
console.log(squares);

const people = [{ id: 1, name: "felpie", country: "USA" },
{ id: 2, name: "mohan", country: "INDIA" },
{ id: 3, name: "jagdish", country: "SRILanka" }
]

const names = people.map(p => p.name);
console.log(names);

// filter function
//array.filter(ele=>(condition))

var filtered = numbers.filter(x => x > 3);
console.log(filtered);